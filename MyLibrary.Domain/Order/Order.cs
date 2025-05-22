using MyLibrary.Domain.Abstraction.Entity;
using MyLibrary.Domain.Helpers;
using MyLibrary.Domain.Order.DomainEvents;
using NodaTime;

namespace MyLibrary.Domain.Order;

public class Order : Entity
{
    public List<OrderItem> Items { get; private set; } = [];
    public Guid? ItemsOwner { get; private set; }
    public Guid Renter { get; init; } = Guid.Empty;
    public OrderStatus Status { get; private set; }
    public LocalDateTime? PickUpDateTime { get; private set; } //check offset datetime
    public LocalDate? PlannedReturnDate { get; private set; }
    public string? Note { get; private set; }

    private Order()
    {
    }

    private Order(List<OrderItem> items, Guid? itemsOwner, Guid renter, OrderStatus status, LocalDateTime? pickUpDateTime, LocalDate? plannedReturnDate, string? note)
    {
        Items = items;
        ItemsOwner = itemsOwner;
        Renter = renter;
        Status = status;
        PickUpDateTime = pickUpDateTime;
        PlannedReturnDate = plannedReturnDate;
        Note = note;
    }

    public static Order CreateEmpty(Guid renter)
    {
        var order = new Order([], null, renter, OrderStatus.CREATED, null, null, null);
        RaiseDomainEvent(new OrderCreated(order.Id, renter));
        return order;
    }

    public void AddItem(OrderItem item)
    {
        if (!IsUpdatePossible())
            throw new InvalidOperationException("Can not 'add item' to order. Order must be 'created' or 'placed'.");

        if (IsOrderEmpty())
        {
            SetOwner(item.Owner);
        }
        else
        {
            if (Items.Contains(item))
                throw new InvalidOperationException("Can not 'add item' to order. Item is already in order.");

            if (!item.Owner.Equals(ItemsOwner))
                throw new InvalidOperationException("Can not 'add item' to order. All items must have same owner.");
        }

        Items.Add(item);
        RaiseDomainEvent(new ItemAddedToOrder(Id, item.ItemId, Renter));
    }

    public void RemoveItem(Guid itemId)
    {
        var orderItem = Items.FirstOrDefault(x => x.ItemId == itemId) ?? throw new InvalidOperationException("Item not found in order.");
        RemoveItem(orderItem);
    }

    public void RemoveItem(OrderItem item)
    {
        if (!IsUpdatePossible())
            throw new InvalidOperationException("Can not 'remove item' from order. Order must be 'created' or 'placed'.");

        Items.Remove(item); //TODO: mozne problemy s trackovanim EF Core
        if (IsOrderEmpty()) SetOwner(null);

        RaiseDomainEvent(new ItemRemovedFromOrder(Id, item.ItemId));
    }

    private void SetOwner(Guid? newOwner)
    {
        if (newOwner is null && !IsOrderEmpty())
            throw new InvalidOperationException("Can not remove owner of items. Items in order are not empty.");

        ItemsOwner = newOwner;
    }

    public void Place(LocalDateTime pickUpDateTime, LocalDate? plannedReturnDate, string? note)
    {
        if (Status is not (OrderStatus.CREATED or OrderStatus.PENDING))
            throw new InvalidOperationException("Can not 'place' order. Order must be 'created' or 'pending'.");

        if (IsOrderEmpty())
            throw new InvalidOperationException("Can not 'place' order. Order must not be empty.");

        SetPickUpDateTime(pickUpDateTime);
        SetPlannedReturnDateTime(plannedReturnDate);
        
        Note = note;
        
        Place();
    }

    private void Place()
    {
        SetOrderStatus(OrderStatus.PLACED);
        RaiseDomainEvent(new OrderPlaced(Id, PickUpDateTime!.Value));  
    }
    
    public void UpdatePickUpDateTime(LocalDateTime pickUpDateTime)
    {
        if (Status is not (OrderStatus.PLACED or OrderStatus.CONFIRMED))
            throw new InvalidOperationException("Can not 'update pick up date time'. Order must be 'placed'.");
        
        SetPickUpDateTime(pickUpDateTime);

        if (Status is OrderStatus.CONFIRMED) Place();
        
        RaiseDomainEvent(new OrderPickUpDateTimeUpdated(Id, PickUpDateTime!.Value));
    }

    private void SetPickUpDateTime(LocalDateTime pickUpDateTime)
    {
        if (NodaTimeHelpers.Now() >= pickUpDateTime)
            throw new InvalidOperationException("Can not 'set pick up date time'. Pick up date time must be in the future.");

        PickUpDateTime = pickUpDateTime;
    }

    private void SetPlannedReturnDateTime(LocalDate? plannedReturnDate)
    {
        if (plannedReturnDate is null)
        {
            PlannedReturnDate = plannedReturnDate;
            return;
        }

        if (NodaTimeHelpers.Today() >= plannedReturnDate)
            throw new InvalidOperationException("Can not 'set planned return date time'. Planned return date time must be in the future.");

        if (plannedReturnDate <= PickUpDateTime?.Date)
            throw new InvalidOperationException("Can not set 'planned return date time'. Planned return date time must be later than pick up date.");

        PlannedReturnDate = plannedReturnDate;
    }

    public void Confirm()
    {
        if (Status is not OrderStatus.PLACED)
            throw new InvalidOperationException("Can not 'confirm' order. Order must be 'placed'."); //TODO: Custom exception? etc. InvalidOrderStatusException

        if (PickUpDateTime is null)
            throw new InvalidOperationException("Can not 'confirm' order. Pick up date time must not be null.");

        SetOrderStatus(OrderStatus.CONFIRMED);
        RaiseDomainEvent(new OrderConfirmed(Id, Items.Select(x => x.ItemId).ToArray(), Renter, PlannedReturnDate));

        //TODO (In application layer): Notify renter that order was confirmed "some-how"
    }
    
    //TODO: DECLINE ORDER

    public void AwaitPickup()
    {
        if (Status is not OrderStatus.CONFIRMED)
            throw new InvalidOperationException("Can not 'await pickup' of order. Order must be 'confirmed'.");

        if (PickUpDateTime is null)
            throw new InvalidOperationException("Can not 'await pickup' order. Pick up date time must not be null.");

        SetOrderStatus(OrderStatus.AWAITING_PICKUP);
        RaiseDomainEvent(new OrderAwaitingPickup(Id));
        //TODO (In application layer): Notify renter and owner that order is awaiting pickup "some-how"
    }

    public void Pickup()
    {
        if (Status is not OrderStatus.AWAITING_PICKUP)
            throw new InvalidOperationException("Can not 'pick up' order. Order must be 'awaiting pickup'.");

        SetOrderStatus(OrderStatus.PICKED_UP);
        RaiseDomainEvent(new OrderPickedUp(Id, Items.Select(x => x.ItemId).ToArray(), Renter));
    }

    public void Complete()
    {
        if (Status is not OrderStatus.PICKED_UP)
            throw new InvalidOperationException("Can not 'complete' order. Order must be 'picked up'.");

        SetOrderStatus(OrderStatus.COMPLETED);
        RaiseDomainEvent(new OrderCompleted(Id, Items.Select(x => x.ItemId).ToArray()));
    }

    public void Cancel()
    {
        if (Status is OrderStatus.COMPLETED)
            throw new InvalidOperationException("'Completed' order can not be 'canceled'.");

        // if (!IsOrderEmpty())
        // {
        //     //TODO (In application layer): Notify owner that order was cancelled "some-how"
        // }

        //Do not reset (delete) items in order to allow user to recreate it from history later.
        SetOrderStatus(OrderStatus.CANCELED);
        RaiseDomainEvent(new OrderCanceled(Id, Items.Select(x => x.ItemId).ToArray()));
    }

    public void ReCreate()
    {
        if (Status is not OrderStatus.CANCELED)
            throw new InvalidOperationException("Item can not be 're-created'. It is not in status 'canceled'");

        SetOrderStatus(OrderStatus.CREATED);
        RaiseDomainEvent(new OrderCreated(Id, Renter));

        foreach (var item in Items)
        {
            RaiseDomainEvent(new ItemAddedToOrder(Id, item.ItemId, Renter));
        }
    }

    private void SetOrderStatus(OrderStatus orderStatus) => Status = orderStatus;

    private bool IsUpdatePossible() => Status is OrderStatus.CREATED or OrderStatus.PENDING;

    private bool IsOrderEmpty() => Items.Count == 0;
}