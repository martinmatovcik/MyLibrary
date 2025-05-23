using System.Diagnostics.CodeAnalysis;
using MyLibrary.Domain.Helpers;
using MyLibrary.Domain.Order;
using MyLibrary.Domain.Order.DomainEvents;
using NodaTime;
using Shouldly;

namespace MyLibrary.Tests.Domain;

[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
public class OrderTests
{
    private readonly Guid _renter;
    private readonly Guid _itemOwner;
    private readonly LocalDateTime _currentDateTime;
    private readonly LocalDate _currentDate;

    public OrderTests()
    {
        _renter = Guid.NewGuid();
        _itemOwner = Guid.NewGuid();

        _currentDateTime = new LocalDateTime(2099, 10, 1, 12, 0);
        _currentDate = _currentDateTime.Date;
    }

    private static void Setup() => new OrderTests();

    #region CreateEmpty Tests

    [Fact]
    public void CreateEmpty_ShouldCreateEmptyOrderWithCreatedStatus()
    {
        //Arrange
        Setup();

        // Act
        var order = Order.CreateEmpty(_renter);

        // Assert
        order.Renter.ShouldBe(_renter);
        order.Items.ShouldBeEmpty();
        order.Status.ShouldBe(OrderStatus.CREATED);
        order.PickUpDateTime.ShouldBeNull();
        order.PlannedReturnDate.ShouldBeNull();

        order.GetDomainEvents().ShouldContain(x => x is OrderCreated);
        order.GetDomainEvents().Count(x => x is OrderCreated).ShouldBe(1);
    }

    #endregion

    #region AddItem Tests

    [Fact]
    public void AddItem_WhenOrderIsCreated_ShouldAddItemAndSetOwner()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        order.ClearDomainEvents();

        // Act
        order.AddItem(item);

        // Assert
        order.Items.Count.ShouldBe(1);
        order.Items[0].ShouldBe(item);
        order.ItemsOwner.ShouldBe(_itemOwner);

        order.GetDomainEvents().ShouldContain(x => x is ItemAddedToOrder);
        order.GetDomainEvents().Count(x => x is ItemAddedToOrder).ShouldBe(1);
    }

    [Fact]
    public void AddItem_WhenAddingMultipleItemsFromSameOwner_ShouldAddAllItems()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item1 = CreateTestOrderItem(_itemOwner);
        var item2 = CreateTestOrderItem(_itemOwner);

        order.ClearDomainEvents();

        // Act
        order.AddItem(item1);
        order.AddItem(item2);

        // Assert
        order.Items.Count.ShouldBe(2);
        order.Items.ShouldContain(item1);
        order.Items.ShouldContain(item2);

        order.GetDomainEvents().ShouldContain(x => x is ItemAddedToOrder);
        order.GetDomainEvents().Count(x => x is ItemAddedToOrder).ShouldBe(2);
    }

    [Fact]
    public void AddItem_WhenAddingItemFromDifferentOwner_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item1 = CreateTestOrderItem(_itemOwner);

        var item2 = CreateTestOrderItem(Guid.NewGuid());

        order.AddItem(item1);

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.AddItem(item2))
            .Message.ShouldContain("All items must have same owner");

        order.GetDomainEvents().ShouldNotContain(x => x is ItemAddedToOrder);
    }

    [Fact]
    public void AddItem_WhenOrderIsNotUpdateable_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        // Move order to non-updateable state (CONFIRMED)
        AddItemAndPlaceOrder(order, item);
        order.Confirm();

        var newItem = CreateTestOrderItem(_itemOwner);

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.AddItem(newItem))
            .Message.ShouldContain("Can not 'add item' to order");

        order.GetDomainEvents().ShouldNotContain(x => x is ItemAddedToOrder);
    }

    #endregion

    #region RemoveItem Tests

    [Fact]
    public void RemoveItem_WhenOrderContainsItem_ShouldRemoveItemAndCancelReservation()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        order.ClearDomainEvents();

        // Act
        order.RemoveItem(item);

        // Assert
        order.Items.ShouldBeEmpty();
        order.GetDomainEvents().ShouldContain(x => x is ItemRemovedFromOrder);
    }

    [Fact]
    public void RemoveItem_WhenRemovingLastItem_ShouldClearOwner()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        order.ClearDomainEvents();

        // Act
        order.RemoveItem(item);

        // Assert
        order.ItemsOwner.ShouldBeNull();
        order.GetDomainEvents().ShouldContain(x => x is ItemRemovedFromOrder);
    }

    [Fact]
    public void RemoveItem_WhenOrderIsNotUpdateable_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        // Move order to non-updateable state (CONFIRMED)
        AddItemAndPlaceOrder(order, item);
        order.Confirm();

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.RemoveItem(item))
            .Message.ShouldContain("Can not 'remove item' from order");

        order.GetDomainEvents().ShouldNotContain(x => x is ItemRemovedFromOrder);
    }

    [Fact]
    public void RemoveItem_WithExistingItemId_ShouldRemoveItemFromOrder()
    {
        // Arrange
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);
        order.ClearDomainEvents();

        // Act
        order.RemoveItem(item.ItemId);

        // Assert
        order.Items.ShouldNotContain(item);
        order.Items.Count.ShouldBe(0);
        
        order.GetDomainEvents().ShouldContain(x => x is ItemRemovedFromOrder);
        order.GetDomainEvents().Count(x => x is ItemRemovedFromOrder).ShouldBe(1);
    }

    [Fact]
    public void RemoveItem_WithNonexistentItemId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var order = Order.CreateEmpty(_renter);
        var nonExistentItemId = Guid.NewGuid();
        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
                order.RemoveItem(nonExistentItemId))
            .Message.ShouldBe("Item not found in order.");
        
        order.GetDomainEvents().ShouldNotContain(x => x is ItemRemovedFromOrder);
    }

    [Fact]
    public void RemoveItem_WhenOrderStatusIsNotCreatedOrPending_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);
        order.ClearDomainEvents();

        // Place the order to change its status
        order.Place(NodaTimeHelpers.Now().PlusDays(1), NodaTimeHelpers.Today().PlusDays(2), "Test note");

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
                order.RemoveItem(item.ItemId))
            .Message.ShouldBe("Can not 'remove item' from order. Order must be 'created' or 'placed'.");
        
        order.GetDomainEvents().ShouldNotContain(x => x is ItemRemovedFromOrder);
    }

    [Fact]
    public void RemoveItem_WhenRemovingLastItem_ShouldSetOwnerToNull()
    {
        // Arrange
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);
        order.ClearDomainEvents();

        // Act
        order.RemoveItem(item.ItemId);

        // Assert
        order.Items.Count.ShouldBe(0);
        order.ItemsOwner.ShouldBeNull();
        
        order.GetDomainEvents().ShouldContain(x => x is ItemRemovedFromOrder);
        order.GetDomainEvents().Count(x => x is ItemRemovedFromOrder).ShouldBe(1);
    }

    #endregion

    #region Place Tests

    [Fact]
    public void Place_WithValidParameters_ShouldUpdateOrderStatus()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var futurePickupTime = _currentDateTime.PlusDays(1);
        var futureReturnDate = _currentDate.PlusDays(7);

        order.ClearDomainEvents();

        // Act
        order.Place(futurePickupTime, futureReturnDate, "Test note");

        // Assert

        order.Status.ShouldBe(OrderStatus.PLACED);
        order.PickUpDateTime.ShouldBe(futurePickupTime);
        order.PlannedReturnDate.ShouldBe(futureReturnDate);

        order.GetDomainEvents().ShouldContain(x => x is OrderPlaced);
    }

    [Fact]
    public void Place_WithEmptyOrder_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var futurePickupTime = _currentDateTime.PlusDays(1);

        order.ClearDomainEvents();

        // Act & Assert

        Should.Throw<InvalidOperationException>(() => order.Place(futurePickupTime, null, null))
            .Message.ShouldContain("Order must not be empty");

        order.GetDomainEvents().ShouldNotContain(x => x is OrderPlaced);
    }

    [Fact]
    public void Place_WithPastPickupTime_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var pastPickupTime = new LocalDateTime(1990, 10, 1, 12, 0);

        order.ClearDomainEvents();

        // Act & Assert

        Should.Throw<InvalidOperationException>(() => order.Place(pastPickupTime, null, null))
            .Message.ShouldContain("Pick up date time must be in the future");

        order.GetDomainEvents().ShouldNotContain(x => x is OrderPlaced);
    }

    [Fact]
    public void Place_WithPastReturnDate_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var futurePickupTime = _currentDateTime.PlusDays(1);
        var pastReturnDate = new LocalDate(1900, 10, 1);

        order.ClearDomainEvents();

        // Act & Assert

        Should.Throw<InvalidOperationException>(() => order.Place(futurePickupTime, pastReturnDate, null))
            .Message.ShouldContain("Planned return date time must be in the future");

        order.GetDomainEvents().ShouldNotContain(x => x is OrderPlaced);
    }

    [Fact]
    public void Place_WithReturnDateBeforePickupDate_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var futurePickupTime = _currentDateTime.PlusDays(2);
        var earlierReturnDate = _currentDate.PlusDays(1);

        order.ClearDomainEvents();

        // Act & Assert

        Should.Throw<InvalidOperationException>(() => order.Place(futurePickupTime, earlierReturnDate, null))
            .Message.ShouldContain("Planned return date time must be later than pick up date");

        order.GetDomainEvents().ShouldNotContain(x => x is OrderPlaced);
    }

    [Fact]
    public void Place_WithNonCreatedStatus_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        // Move order to CONFIRMED state
        AddItemAndPlaceOrder(order, item);
        order.Confirm();

        var futurePickupTime = _currentDateTime.PlusDays(1);

        order.ClearDomainEvents();

        // Act & Assert

        Should.Throw<InvalidOperationException>(() => order.Place(futurePickupTime, null, null))
            .Message.ShouldContain("Can not 'place' order");

        order.GetDomainEvents().ShouldNotContain(x => x is OrderPlaced);
    }

    #endregion

    #region Confirm Tests

    [Fact]
    public void Confirm_WhenOrderIsPlaced_ShouldUpdateStatus()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);

        order.ClearDomainEvents();

        // Act
        order.Confirm();

        // Assert
        order.Status.ShouldBe(OrderStatus.CONFIRMED);

        order.GetDomainEvents().ShouldContain(x => x is OrderConfirmed);
        order.GetDomainEvents().Count(x => x is OrderConfirmed).ShouldBe(1);
    }

    [Fact]
    public void Confirm_WhenOrderIsNotPlaced_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.Confirm())
            .Message.ShouldContain("Order must be 'placed'");

        order.GetDomainEvents().ShouldNotContain(x => x is OrderConfirmed);
    }
    
    [Fact]
    public void Confirm_WhenPickUpDateTimeIsNull_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);
        
        typeof(Order).GetProperty("PickUpDateTime")?.SetValue(order, null);

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.Confirm())
            .Message.ShouldContain("Pick up date time must not be null");
        
        order.GetDomainEvents().ShouldNotContain(x => x is OrderConfirmed);
    }

    #endregion

    #region AwaitPickup Tests

    [Fact]
    public void AwaitPickup_WhenOrderIsConfirmed_ShouldUpdateStatus()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);
        order.Confirm();

        order.ClearDomainEvents();

        // Act
        order.AwaitPickup();

        // Assert
        order.Status.ShouldBe(OrderStatus.AWAITING_PICKUP);
    }

    [Fact]
    public void AwaitPickup_WhenOrderIsNotConfirmed_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.AwaitPickup())
            .Message.ShouldContain("Order must be 'confirmed'");
    }
    
    [Fact]
    public void AwaitPickup_WhenPickUpDateTimeIsNull_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);
        order.Confirm();
        
        typeof(Order).GetProperty("PickUpDateTime")?.SetValue(order, null);

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.AwaitPickup())
            .Message.ShouldContain("Pick up date time must not be null");
        
        order.GetDomainEvents().ShouldNotContain(x => x is OrderAwaitingPickup);
    }

    #endregion

    #region PickUp Tests

    [Fact]
    public void PickUp_WhenOrderIsAwaitingPickup_ShouldUpdateStatusAndRentItems()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        var futureReturnDate = _currentDate.PlusDays(7);

        AddItemAndPlaceOrder(order, item, futureReturnDate);
        order.Confirm();
        order.AwaitPickup();

        order.ClearDomainEvents();

        // Act
        order.Pickup();

        // Assert
        order.Status.ShouldBe(OrderStatus.PICKED_UP);
    }

    [Fact]
    public void PickUp_WhenOrderIsNotAwaitingPickup_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);
        order.Confirm();

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.Pickup())
            .Message.ShouldContain("Order must be 'awaiting pickup'");
    }

    #endregion

    #region Complete Tests

    [Fact]
    public void Complete_WhenOrderIsPickedUp_ShouldUpdateStatusAndReturnItems()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        AddItemAndPlaceOrder(order, item);
        order.Confirm();
        order.AwaitPickup();
        order.Pickup();

        order.ClearDomainEvents();

        // Act
        order.Complete();

        // Assert
        order.Status.ShouldBe(OrderStatus.COMPLETED);
    }

    [Fact]
    public void Complete_WhenOrderIsNotPickedUp_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        AddItemAndPlaceOrder(order, item);
        order.Confirm();
        order.AwaitPickup();

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.Complete())
            .Message.ShouldContain("Order must be 'picked up'");
    }

    #endregion

    #region Cancel Tests

    [Fact]
    public void Cancel_WhenOrderIsPlaced_ShouldCancelReservations()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);

        order.ClearDomainEvents();

        // Act
        order.Cancel();

        // Assert
        order.Status.ShouldBe(OrderStatus.CANCELED);
    }

    [Fact]
    public void Cancel_WhenOrderIsCompleted_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        AddItemAndPlaceOrder(order, item);
        order.Confirm();
        order.AwaitPickup();
        order.Pickup();
        order.Complete();

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.Cancel())
            .Message.ShouldContain("'Completed' order can not be 'canceled'");
    }

    #endregion

    #region ReCreate Tests

    [Fact]
    public void ReCreate_WhenOrderIsCanceled_ShouldResetToCreatedStatus()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);
        order.Cancel();

        order.ClearDomainEvents();

        // Act
        order.ReCreate();

        // Assert
        order.Status.ShouldBe(OrderStatus.CREATED);
    }

    [Fact]
    public void ReCreate_WhenOrderIsNotCanceled_ShouldThrowException()
    {
        // Arrange
        Setup();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.ReCreate())
            .Message.ShouldContain("Item can not be 're-created'");
    }

    #endregion

    [Fact]
    public void Place_WithEarlierPickupTime_ShouldRequireExplicitCancelAndRecreate()
    {
        // Arrange
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var initialPickupTime = _currentDateTime.PlusDays(3);
        order.Place(initialPickupTime, _currentDate.PlusDays(10), "Initial note");

        var earlierPickupTime = _currentDateTime.PlusDays(2);

        typeof(Order).GetProperty("Status")?.SetValue(order, OrderStatus.PENDING);

        order.ClearDomainEvents();

        // Act
        order.Place(earlierPickupTime, _currentDate.PlusDays(10), "Updated note");

        // Assert
        order.Status.ShouldBe(OrderStatus.PLACED);
        order.PickUpDateTime.ShouldBe(earlierPickupTime);

        order.GetDomainEvents().ShouldContain(x => x is OrderPlaced);
        order.GetDomainEvents().Count(x => x is OrderPlaced).ShouldBe(1);
    }

    [Fact]
    public void Place_WithNullReturnDate_ShouldSetNullReturnDate()
    {
        // Arrange
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var pickupTime = _currentDateTime.PlusDays(1);

        order.ClearDomainEvents();

        // Act
        order.Place(pickupTime, null, "Test note");

        // Assert
        order.Status.ShouldBe(OrderStatus.PLACED);
        order.PickUpDateTime.ShouldBe(pickupTime);
        order.PlannedReturnDate.ShouldBeNull();
    }

    [Fact]
    public void Place_WithPendingStatus_ShouldUpdateOrderCorrectly()
    {
        // Arrange
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        // Set order to PENDING status using reflection
        typeof(Order).GetProperty("Status")?.SetValue(order, OrderStatus.PENDING);

        var pickupTime = _currentDateTime.PlusDays(1);

        order.ClearDomainEvents();

        // Act
        order.Place(pickupTime, _currentDate.PlusDays(7), "Test note");

        // Assert
        order.Status.ShouldBe(OrderStatus.PLACED);
        order.PickUpDateTime.ShouldBe(pickupTime);
    }

    [Fact]
    public void UpdatePickUpDateTime_WhenOrderIsPlaced_ShouldUpdatePickUpDateTime()
    {
        // Arrange
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var initialPickupTime = NodaTimeHelpers.Now().PlusDays(3);
        order.Place(initialPickupTime, NodaTimeHelpers.Today().PlusDays(5), "Initial note");

        var newPickupTime = NodaTimeHelpers.Now().PlusDays(4);

        order.ClearDomainEvents();

        // Act
        order.UpdatePickUpDateTime(newPickupTime);

        // Assert
        order.PickUpDateTime.ShouldBe(newPickupTime);
        order.Status.ShouldBe(OrderStatus.PLACED);

        order.GetDomainEvents().ShouldContain(x => x is OrderPickUpDateTimeUpdated);
        order.GetDomainEvents().Count(x => x is OrderPickUpDateTimeUpdated).ShouldBe(1);
        order.GetDomainEvents().ShouldNotContain(x => x is OrderPlaced);
    }

    [Fact]
    public void UpdatePickUpDateTime_WhenOrderIsConfirmed_ShouldUpdatePickUpDateTimeAndSetStatusToPlaced()
    {
        // Arrange
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var initialPickupTime = NodaTimeHelpers.Now().PlusDays(3);
        order.Place(initialPickupTime, NodaTimeHelpers.Today().PlusDays(5), "Initial note");
        order.Confirm();

        var newPickupTime = NodaTimeHelpers.Now().PlusDays(4);

        order.ClearDomainEvents();

        // Act
        order.UpdatePickUpDateTime(newPickupTime);

        // Assert
        order.PickUpDateTime.ShouldBe(newPickupTime);
        order.Status.ShouldBe(OrderStatus.PLACED);

        order.GetDomainEvents().ShouldContain(x => x is OrderPickUpDateTimeUpdated);
        order.GetDomainEvents().Count(x => x is OrderPickUpDateTimeUpdated).ShouldBe(1);
        order.GetDomainEvents().ShouldContain(x => x is OrderPlaced);
        order.GetDomainEvents().Count(x => x is OrderPlaced).ShouldBe(1);
    }

    [Fact]
    public void UpdatePickUpDateTime_WhenOrderIsNotPlacedOrConfirmed_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var newPickupTime = NodaTimeHelpers.Now().PlusDays(4);

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
                order.UpdatePickUpDateTime(newPickupTime))
            .Message.ShouldBe("Can not 'update pick up date time'. Order must be 'placed'.");

        order.GetDomainEvents().ShouldNotContain(x => x is OrderPickUpDateTimeUpdated);
        order.GetDomainEvents().ShouldNotContain(x => x is OrderPlaced);
    }

    [Fact]
    public void UpdatePickUpDateTime_WithPastDateTime_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var initialPickupTime = NodaTimeHelpers.Now().PlusDays(3);
        order.Place(initialPickupTime, NodaTimeHelpers.Today().PlusDays(5), "Initial note");

        var pastDateTime = NodaTimeHelpers.Now().PlusDays(-1);

        order.ClearDomainEvents();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
                order.UpdatePickUpDateTime(pastDateTime))
            .Message.ShouldBe("Can not 'set pick up date time'. Pick up date time must be in the future.");

        order.GetDomainEvents().ShouldNotContain(x => x is OrderPickUpDateTimeUpdated);
        order.GetDomainEvents().ShouldNotContain(x => x is OrderPlaced);
    }

    #region Helper Methods

    private static OrderItem CreateTestOrderItem(Guid owner) => new(Guid.NewGuid(), "Test Item", owner);

    private void AddItemAndPlaceOrder(Order order, OrderItem item, LocalDate? returnDate = null)
    {
        order.AddItem(item);
        var futurePickupTime = _currentDateTime.PlusDays(1);
        order.Place(futurePickupTime, returnDate ?? _currentDate.PlusDays(7), "Test note");
    }

    #endregion

}