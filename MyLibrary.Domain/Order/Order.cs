using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Helpers;
using MyLibrary.Domain.User;
using NodaTime;

namespace MyLibrary.Domain.Order;

public class Order : Entity
{
    public List<Item.Item> Items { get; private set; } = [];
    private LibraryUser? ItemsOwner { get; set; }
    public LibraryUser Renter { get; init; } = LibraryUser.CreateEmpty();
    public OrderStatus Status { get; private set; }
    public LocalDateTime? PickUpDateTime { get; private set; }

    private Order()
    {
    }

    private Order(List<Item.Item> items, LibraryUser? itemsOwner, LibraryUser renter, OrderStatus status, LocalDateTime? pickUpDateTime)
    {
        Items = items;
        ItemsOwner = itemsOwner;
        Renter = renter;
        Status = status;
        PickUpDateTime = pickUpDateTime;
    }

    public static Order CreateEmpty(LibraryUser renter) => new([], null, renter, OrderStatus.CREATED, null);

    public void AddItem(Item.Item item)
    {
        if (!IsUpdatePossible())
            throw new InvalidOperationException("Can not 'add item' to order. Order must be 'created' or 'placed'.");

        if (IsEmpty())
        {
            Items.Add(item);
            SetOwner(item.Owner);
            return;
        }

        if (!item.Owner.Equals(ItemsOwner))
            throw new InvalidOperationException("Can not 'add item' to order. All items must have same owner.");

        Items.Add(item);
    }

    public void RemoveItem(Item.Item item)
    {
        if (!IsUpdatePossible())
            throw new InvalidOperationException("Can not 'remove item' from order. Order must be 'created' or 'placed'.");

        Items.Remove(item);
        if (IsEmpty()) SetOwner(null);
    }

    private void SetOwner(LibraryUser? newOwner)
    {
        if (newOwner is null && !IsEmpty())
            throw new InvalidOperationException("Can not remove owner of items. Items in order are not empty.");

        if (newOwner is not null && IsEmpty())
            throw new InvalidOperationException("Can not set owner of items. Items in order are empty.");

        ItemsOwner = newOwner;
    }

    public void Place(LocalDateTime pickUpDateTime)
    {
        if (Status is not (OrderStatus.CREATED or OrderStatus.PENDING))
            throw new InvalidOperationException("Can not 'place' order. Order must be 'created' or 'placed'.");

        if (IsEmpty())
            throw new InvalidOperationException("Can not 'place' order. Order must not be empty.");

        SetPickUpDateTime(pickUpDateTime);
        Place();
    }

    private void Place()
    {
        SetOrderStatus(OrderStatus.PLACED);

        //TODO: Notify owner to confirm order "some-how"
    }

    public void Confirm()
    {
        if (Status is not OrderStatus.PLACED)
            throw new InvalidOperationException("Can not 'confirm' order. Order must be 'placed'."); //TODO: Custom exception? etc. InvalidOrderStatusException

        if (PickUpDateTime is null)
            throw new InvalidOperationException("Can not 'confirm' order. Pick up date time must not be null.");

        SetOrderStatus(OrderStatus.CONFIRMED);

        //TODO: Notify renter that order was confirmed "some-how"
    }

    public void AwaitPickup()
    {
        if (Status is not OrderStatus.CONFIRMED)
            throw new InvalidOperationException("Can not 'await pickup' of order. Order must be 'confirmed'.");

        if (PickUpDateTime is null)
            throw new InvalidOperationException("Can not 'await pickup' order. Pick up date time must not be null.");

        SetOrderStatus(OrderStatus.AWAITING_PICKUP);

        //TODO: Notify renter and owner that order is awaiting pickup "some-how"
    }

    public void Cancel()
    {
        if (!IsEmpty())
        {
            //TODO: Notify owner that order was cancelled "some-how"
        }

        //Do not reset items in order to allow user to recreate it from history later.

        SetOrderStatus(OrderStatus.CANCELED);
    }

    public void PickUp()
    {
        if (Status is not OrderStatus.AWAITING_PICKUP)
            throw new InvalidOperationException("Can not 'pick up' order. Order must be 'awaiting pickup'.");

        SetOrderStatus(OrderStatus.PICKED_UP);

        //TODO: Rental detail update
    }

    public void Complete()
    {
        if (Status is not OrderStatus.PICKED_UP)
            throw new InvalidOperationException("Can not 'complete' order. Order must be 'picked up'.");

        SetOrderStatus(OrderStatus.COMPLETED);

        //TODO: Rental detail update
        //TODO: Return items
    }

    public void SetPickUpDateTime(LocalDateTime pickUpDateTime)
    {
        if (NodaTimeHelpers.Now() >= pickUpDateTime)
            throw new InvalidOperationException("Can not 'set pick up date time'. Pick up date time must be in the future.");

        if (pickUpDateTime < PickUpDateTime)
        {
            Cancel();
            Place();
        }

        PickUpDateTime = pickUpDateTime;
    }

    private void SetOrderStatus(OrderStatus orderStatus) => Status = orderStatus;

    private bool IsUpdatePossible() => Status is OrderStatus.CREATED or OrderStatus.PENDING;

    private bool IsEmpty() => Items.Count == 0;
}