using MyLibrary.Domain.Item.Abstraction;
using NodaTime;
using Shouldly;

namespace MyLibrary.Tests.Domain;

public class ItemTests
{
    private class TestItem : Item
    {
        private TestItem(string name, string? description, Guid owner, Guid? renter, List<RentalDetail> history, ItemStatus status)
            : base(name, description, owner, renter, history, status)
        {
        }

        static internal TestItem CreateWithStatus(ItemStatus itemStatus) => new("test-item", "test-description", Guid.NewGuid(), null, [], itemStatus);

        static internal TestItem CreateWithStatusAndRenter(ItemStatus itemStatus) =>
            new("test-item", "test-description", Guid.NewGuid(), Guid.NewGuid(), [], itemStatus);

        internal void CreateNotReturnedHistory(Guid renter, LocalDate? plannedReturnDate) => History.Add(RentalDetail.CreateActive(renter, plannedReturnDate));
    }

    [Fact]
    public void Reserve_WhenItemIsAvailable_ShouldSetStatusToReservedAndRenterCorrectly()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var renter = Guid.NewGuid();
        
        // Act
        item.Reserve(renter);

        // Assert
        item.Status.ShouldBe(ItemStatus.RESERVED);
        item.Renter.ShouldBe(renter);
    }

    [Fact]
    public void Reserve_WithNotReturnedItem_ShouldThrowException()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var renter = Guid.NewGuid();

        item.Rent(renter, null);
        typeof(Item).GetProperty("Status")!.SetValue(item, ItemStatus.AVAILABLE);

        // Assert
        Should.Throw<InvalidOperationException>(() => item.Reserve(renter))
            .Message.ShouldBe("Item cannot be 'reserved' because it has not been returned.");
    }

    [Fact]
    public void Reserve_WhenItemIsNotAvailable_ShouldThrowException()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.NOT_AVAILABLE);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => item.Reserve(Guid.NewGuid()))
            .Message.ShouldBe("Item cannot be 'reserved' because it is not available.");
    }

    [Fact]
    public void CancelReservation_WhenItemIsNotReserved_ShouldThrowException()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);

        // Assert
        Should.Throw<InvalidOperationException>(() => item.CancelReservation())
            .Message.ShouldBe("Can not 'cancel reservation' because item is not reserved.");
    }

    [Fact]
    public void CancelReservation_WhenItemIsReserved_ShouldSetStatusToAvailable()
    {
        // Arrange
        var item = TestItem.CreateWithStatusAndRenter(ItemStatus.RESERVED);

        // Act
        item.CancelReservation();

        // Assert
        item.Status.ShouldBe(ItemStatus.AVAILABLE);
    }

    [Fact]
    public void Rent_WhenItemIsNotAvailable_ShouldThrowException()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var renter = Guid.NewGuid();
        
        item.CreateNotReturnedHistory(renter, null);
        
        // Assert
        Should.Throw<InvalidOperationException>(() => item.Rent(renter, null))
            .Message.ShouldBe("Item cannot be 'rented' because it has not been returned.");
    }

    [Fact]
    public void Rent_WhenNotAvailable_ShouldThrowException()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.NOT_AVAILABLE);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => item.Rent(Guid.NewGuid(), null))
            .Message.ShouldBe("Item cannot be 'rented' because it is not available.");
    }

    [Fact]
    public void Rent_WhenItemIsAvailable_ShouldSetStatusToNotAvailable()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var renter = Guid.NewGuid();

        // Act
        item.Rent(renter, null);

        // Assert
        item.Status.ShouldBe(ItemStatus.NOT_AVAILABLE);
        item.Renter.ShouldBe(renter);
    }

    [Fact]
    public void Rent_WhenItemIsReserved_ShouldSetStatusToNotAvailable()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var renter = Guid.NewGuid();
        item.Reserve(renter);

        // Act
        item.Rent(renter, null);

        // Assert
        item.Status.ShouldBe(ItemStatus.NOT_AVAILABLE);
        item.Renter.ShouldBe(renter);
    }

    [Fact]
    public void Rent_WhenItemIsReservedByDifferentUser_ShouldThrowException()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var reserver = Guid.NewGuid();
        var renter = Guid.NewGuid();

        item.Reserve(reserver);

        // Assert
        Should.Throw<InvalidOperationException>(() => item.Rent(renter, null))
            .Message.ShouldBe("Item can not be 'rented' because it is 'reserved' by different user.");
    }

    [Fact]
    public void Rent_ShouldAddHistoryRecord()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var renter = Guid.NewGuid();
        var returnDate = LocalDate.FromDateTime(DateTime.Now.AddDays(7));

        // Act
        item.Rent(renter, returnDate);

        // Assert
        item.History.Count.ShouldBe(1);
        item.History[0].Status.ShouldBe(RentalDetailStatus.ACTIVE);
        item.History[0].Renter.ShouldBe(renter);
        item.History[0].PlannedReturnDate.ShouldBe(returnDate);
    }

    [Fact]
    public void Return_WhenItemIsRented_ShouldSetStatusToAvailable()
    {
        // Arrange
        var item = TestItem.CreateWithStatusAndRenter(ItemStatus.AVAILABLE);
        var renter = Guid.NewGuid();
        item.Rent(renter, null);

        // Act
        item.Return();

        // Assert
        item.Status.ShouldBe(ItemStatus.AVAILABLE);
    }

    [Fact]
    public void Return_WhenItemIsNotRented_ShouldThrowException()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);

        // Assert
        Should.Throw<InvalidOperationException>(() => item.Return())
            .Message.ShouldBe("Item can not be 'returned' because it has not been 'rented'.");
    }

    [Fact]
    public void Return_ShouldUpdateHistoryRecord()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var renter = Guid.NewGuid();
        item.Rent(renter, null);

        // Act
        item.Return();

        // Assert
        item.History.Count.ShouldBe(1);
        item.History[0].Status.ShouldBe(RentalDetailStatus.COMPLETED);
    }
    
    [Fact]
    public void Return_ShouldSetRenterToNull()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var renter = Guid.NewGuid();
        item.Rent(renter, null);
        
        // Act
        item.Return();
        
        // Assert
        item.Renter.ShouldBeNull();
    }
    
    [Fact]
    public void Return_ShouldSetActualReturnDateInHistoryRecord()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var renter = Guid.NewGuid();
        var today = LocalDateTime.FromDateTime(DateTime.Today);
        item.Rent(renter, today.PlusDays(7).Date);
        
        // Act
        item.Return();
        
        // Assert
        item.History[0].RealReturnDateTime.ShouldNotBeNull();
        item.History[0].RealReturnDateTime!.Value.ShouldBeGreaterThanOrEqualTo(today);
    }
    
    [Fact]
    public void CancelReservation_ShouldSetRenterToNull()
    {
        // Arrange
        var renter = Guid.NewGuid();
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        item.Reserve(renter);
        
        // Act
        item.CancelReservation();
        
        // Assert
        item.Renter.ShouldBeNull();
    }
    
    [Fact] 
    public void Rent_WithPlannedReturnDate_ShouldSetDateCorrectly()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var renter = Guid.NewGuid();
        var returnDate = LocalDate.FromDateTime(DateTime.Now.AddDays(14));
    
        // Act
        item.Rent(renter, returnDate);
    
        // Assert
        item.History[0].PlannedReturnDate.ShouldBe(returnDate);
    }
}