using MyLibrary.Domain.Item.Abstraction;
using MyLibrary.Domain.Order;
using MyLibrary.Domain.User;
using NodaTime;
using Shouldly;

namespace MyLibrary.Tests.Domain;

public class OrderTests
{
    private readonly LibraryUser _renter;
    private readonly Guid _itemOwner;
    private readonly LocalDateTime _currentDateTime;
    private readonly LocalDate _currentDate;

    public OrderTests()
    {
        var renterDetails = new UserDetails("johnDoe", "password123", "John", "Doe", "john.doe@example.com", "555-1234");
        _renter = new LibraryUser(renterDetails, [], []);

        // var ownerDetails = new UserDetails("janeSmith", "password456", "Jane", "Smith", "jane.smith@example.com", "555-5678");
        // _itemOwner = new LibraryUser(ownerDetails, [], []);
        _itemOwner = Guid.NewGuid();

        _currentDateTime = new LocalDateTime(2099, 10, 1, 12, 0);
        _currentDate = _currentDateTime.Date;
    }

    #region CreateEmpty Tests

    [Fact]
    public void CreateEmpty_ShouldCreateEmptyOrderWithCreatedStatus()
    {
        //Arrange
        new OrderTests();

        // Act
        var order = Order.CreateEmpty(_renter);

        // Assert
        order.Renter.ShouldBe(_renter);
        order.Items.ShouldBeEmpty();
        order.Status.ShouldBe(OrderStatus.CREATED);
        order.PickUpDateTime.ShouldBeNull();
        order.PlannedReturnDate.ShouldBeNull();
    }

    #endregion

    #region AddItem Tests

    [Fact]
    public void AddItem_WhenOrderIsCreated_ShouldAddItemAndSetOwner()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        // Act
        order.AddItem(item);

        // Assert
        order.Items.Count.ShouldBe(1);
        order.Items[0].ShouldBe(item);
    }

    [Fact]
    public void AddItem_WhenAddingMultipleItemsFromSameOwner_ShouldAddAllItems()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item1 = CreateTestOrderItem(_itemOwner);
        var item2 = CreateTestOrderItem(_itemOwner);

        // Act
        order.AddItem(item1);
        order.AddItem(item2);

        // Assert
        order.Items.Count.ShouldBe(2);
        order.Items.ShouldContain(item1);
        order.Items.ShouldContain(item2);
    }

    [Fact]
    public void AddItem_WhenAddingItemFromDifferentOwner_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item1 = CreateTestOrderItem(_itemOwner);

        var item2 = CreateTestOrderItem(Guid.NewGuid());

        order.AddItem(item1);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.AddItem(item2))
            .Message.ShouldContain("All items must have same owner");
    }

    [Fact]
    public void AddItem_WhenOrderIsNotUpdateable_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        // Move order to non-updateable state (CONFIRMED)
        AddItemAndPlaceOrder(order, item);
        order.Confirm();

        var newItem = CreateTestOrderItem(_itemOwner);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.AddItem(newItem))
            .Message.ShouldContain("Can not 'add item' to order");
    }

    #endregion

    #region RemoveItem Tests

    [Fact]
    public void RemoveItem_WhenOrderContainsItem_ShouldRemoveItemAndCancelReservation()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        // Act
        order.RemoveItem(item);

        // Assert
        order.Items.ShouldBeEmpty();
    }

    [Fact]
    public void RemoveItem_WhenRemovingLastItem_ShouldClearOwner()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        // Act
        order.RemoveItem(item);

        // Assert
        order.Items.ShouldBeEmpty();

        var differentItem = CreateTestOrderItem(Guid.NewGuid());

        // Should be able to add item from different owner as owner was cleared
        order.AddItem(differentItem);
        order.Items.Count.ShouldBe(1);
        order.Items[0].ShouldBe(differentItem);
    }

    [Fact]
    public void RemoveItem_WhenOrderIsNotUpdateable_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        // Move order to non-updateable state (CONFIRMED)
        AddItemAndPlaceOrder(order, item);
        order.Confirm();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.RemoveItem(item))
            .Message.ShouldContain("Can not 'remove item' from order");
    }

    #endregion

    #region Place Tests

    [Fact]
    public void Place_WithValidParameters_ShouldUpdateOrderStatus()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var futurePickupTime = _currentDateTime.PlusDays(1);
        var futureReturnDate = _currentDate.PlusDays(7);

        // Act
        order.Place(futurePickupTime, futureReturnDate, "Test note");

        // Assert
        order.Status.ShouldBe(OrderStatus.PLACED);
        order.PickUpDateTime.ShouldBe(futurePickupTime);
        order.PlannedReturnDate.ShouldBe(futureReturnDate);
    }

    [Fact]
    public void Place_WithEmptyOrder_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var futurePickupTime = _currentDateTime.PlusDays(1);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.Place(futurePickupTime, null, null))
            .Message.ShouldContain("Order must not be empty");
    }

    [Fact]
    public void Place_WithPastPickupTime_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var pastPickupTime = new LocalDateTime(1990, 10, 1, 12, 0);
        ;

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.Place(pastPickupTime, null, null))
            .Message.ShouldContain("Pick up date time must be in the future");
    }

    [Fact]
    public void Place_WithPastReturnDate_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var futurePickupTime = _currentDateTime.PlusDays(1);
        var pastReturnDate = new LocalDate(1900, 10, 1);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.Place(futurePickupTime, pastReturnDate, null))
            .Message.ShouldContain("Planned return date time must be in the future");
    }

    [Fact]
    public void Place_WithReturnDateBeforePickupDate_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        var futurePickupTime = _currentDateTime.PlusDays(2);
        var earlierReturnDate = _currentDate.PlusDays(1);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.Place(futurePickupTime, earlierReturnDate, null))
            .Message.ShouldContain("Planned return date time must be later than pick up date");
    }

    [Fact]
    public void Place_WithNonCreatedStatus_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        // Move order to CONFIRMED state
        AddItemAndPlaceOrder(order, item);
        order.Confirm();

        var futurePickupTime = _currentDateTime.PlusDays(1);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.Place(futurePickupTime, null, null))
            .Message.ShouldContain("Can not 'place' order");
    }

    #endregion

    #region Confirm Tests

    [Fact]
    public void Confirm_WhenOrderIsPlaced_ShouldUpdateStatus()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);

        // Act
        order.Confirm();

        // Assert
        order.Status.ShouldBe(OrderStatus.CONFIRMED);
    }

    [Fact]
    public void Confirm_WhenOrderIsNotPlaced_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.Confirm())
            .Message.ShouldContain("Order must be 'placed'");
    }

    #endregion

    #region AwaitPickup Tests

    [Fact]
    public void AwaitPickup_WhenOrderIsConfirmed_ShouldUpdateStatus()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);
        order.Confirm();

        // Act
        order.AwaitPickup();

        // Assert
        order.Status.ShouldBe(OrderStatus.AWAITING_PICKUP);
    }

    [Fact]
    public void AwaitPickup_WhenOrderIsNotConfirmed_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.AwaitPickup())
            .Message.ShouldContain("Order must be 'confirmed'");
    }

    #endregion

    #region PickUp Tests

    [Fact]
    public void PickUp_WhenOrderIsAwaitingPickup_ShouldUpdateStatusAndRentItems()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        var futureReturnDate = _currentDate.PlusDays(7);

        AddItemAndPlaceOrder(order, item, futureReturnDate);
        order.Confirm();
        order.AwaitPickup();

        // Act
        order.PickUp();

        // Assert
        order.Status.ShouldBe(OrderStatus.PICKED_UP);
    }

    [Fact]
    public void PickUp_WhenOrderIsNotAwaitingPickup_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);
        order.Confirm();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => order.PickUp())
            .Message.ShouldContain("Order must be 'awaiting pickup'");
    }

    #endregion

    #region Complete Tests

    [Fact]
    public void Complete_WhenOrderIsPickedUp_ShouldUpdateStatusAndReturnItems()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        AddItemAndPlaceOrder(order, item);
        order.Confirm();
        order.AwaitPickup();
        order.PickUp();

        // Act
        order.Complete();

        // Assert
        order.Status.ShouldBe(OrderStatus.COMPLETED);
    }

    [Fact]
    public void Complete_WhenOrderIsNotPickedUp_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        AddItemAndPlaceOrder(order, item);
        order.Confirm();
        order.AwaitPickup();

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
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);

        // Act
        order.Cancel();

        // Assert
        order.Status.ShouldBe(OrderStatus.CANCELED);
    }

    [Fact]
    public void Cancel_WhenOrderIsCompleted_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);

        AddItemAndPlaceOrder(order, item);
        order.Confirm();
        order.AwaitPickup();
        order.PickUp();
        order.Complete();

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
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);
        order.Cancel();

        // Act
        order.ReCreate();

        // Assert
        order.Status.ShouldBe(OrderStatus.CREATED);
    }

    [Fact]
    public void ReCreate_WhenOrderIsNotCanceled_ShouldThrowException()
    {
        // Arrange
        new OrderTests();
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        AddItemAndPlaceOrder(order, item);

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
        
        // Cancel and recreate the order
        order.Cancel();
        order.ReCreate();
        order.AddItem(item);
        
        var earlierPickupTime = _currentDateTime.PlusDays(2);
        
        // Act
        order.Place(earlierPickupTime, _currentDate.PlusDays(10), "Updated note");
        
        // Assert
        order.Status.ShouldBe(OrderStatus.PLACED);
        order.PickUpDateTime.ShouldBe(earlierPickupTime);
    }
    
    [Fact]
    public void Place_WithNullReturnDate_ShouldSetNullReturnDate()
    {
        // Arrange
        var order = Order.CreateEmpty(_renter);
        var item = CreateTestOrderItem(_itemOwner);
        order.AddItem(item);
        
        var pickupTime = _currentDateTime.PlusDays(1);
        
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
        
        // Act
        order.Place(pickupTime, _currentDate.PlusDays(7), "Test note");
        
        // Assert
        order.Status.ShouldBe(OrderStatus.PLACED);
        order.PickUpDateTime.ShouldBe(pickupTime);
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