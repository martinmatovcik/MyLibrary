using MyLibrary.Domain.Item;
using MyLibrary.Domain.User;
using MyLibrary.Tests.Data;
using NodaTime;
using Shouldly;

namespace MyLibrary.Tests.Domain;

public class ItemTests
{
    private class TestItem : Item
    {
        private TestItem(string name, string? description, LibraryUser owner, LibraryUser? renter, List<RentalDetail> history, ItemStatus status)
            : base(name, description, owner, renter, history, status)
        {
        }
        // Concrete implementation of the abstract Item class for testing

        static internal TestItem CreateWithStatus(ItemStatus itemStatus) => new("test-item", "test-description", LibraryUser.CreateEmpty(), null, [], itemStatus);

        static internal TestItem CreateWithStatusAndRenter(ItemStatus itemStatus) =>
            new("test-item", "test-description", LibraryUser.CreateEmpty(), LibraryUser.CreateEmpty(), [], itemStatus);

        internal void CreateNotReturnedHistory(LibraryUser renter, LocalDate? plannedReturnDate) => History.Add(RentalDetail.CreateActive(renter, plannedReturnDate));
    }

    [Fact]
    public void Reserve_WhenItemIsAvailable_ShouldSetStatusToReservedAndRenterCorrectly()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var renter = UserData.CreateTestRenter();

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
        var renter = LibraryUser.CreateEmpty();

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
        Should.Throw<InvalidOperationException>(() => item.Reserve(LibraryUser.CreateEmpty()))
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
        var renter = LibraryUser.CreateEmpty();
        
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
        Should.Throw<InvalidOperationException>(() => item.Rent(LibraryUser.CreateEmpty(), null))
            .Message.ShouldBe("Item cannot be 'rented' because it is not available.");
    }

    [Fact]
    public void Rent_WhenItemIsAvailable_ShouldSetStatusToNotAvailable()
    {
        // Arrange
        var item = TestItem.CreateWithStatus(ItemStatus.AVAILABLE);
        var renter = UserData.CreateTestRenter();

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
        var renter = LibraryUser.CreateEmpty();
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
        var reserver = new LibraryUser(
            new UserDetails("reserver", "password", "First", "Last", "email@example.com", "1234567890"),
            [], []);
        var renter = UserData.CreateTestRenter();

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
        var renter = LibraryUser.CreateEmpty();
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
        var renter = LibraryUser.CreateEmpty();
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
        var renter = LibraryUser.CreateEmpty();
        item.Rent(renter, null);

        // Act
        item.Return();

        // Assert
        item.History.Count.ShouldBe(1);
        item.History[0].Status.ShouldBe(RentalDetailStatus.COMPLETED);
    }
}