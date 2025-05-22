using Moq;
using MyLibrary.Application.Features.Order.RemoveItems;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Order;
using MyLibrary.Domain.Order.Repository;
using Shouldly;

namespace MyLibrary.Tests.Application.Order.RemoveItems;

public class RemoveItemsFromOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly RemoveItemsFromOrderCommandHandler _handler;

    public RemoveItemsFromOrderCommandHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new RemoveItemsFromOrderCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);
    }

    private static void Setup() => new RemoveItemsFromOrderCommandHandlerTests();

    [Fact]
    public async Task Handle_ValidOrderAndItems_RemovesItemsAndReturnsResponse()
    {
        // Arrange
        Setup();
        var orderId = Guid.NewGuid();
        var renterId = Guid.NewGuid();
        var order = MyLibrary.Domain.Order.Order.CreateEmpty(renterId);

        var itemId = Guid.NewGuid();
        order.AddItem(new OrderItem(itemId, "item", Guid.NewGuid()));

        var itemIds = new List<Guid> { itemId };
        var command = new RemoveItemsFromOrderCommand(orderId, itemIds);

        _mockRepository.Setup(r => r.FirstOrDefaultByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldBeEmpty();
        result.RenterId.ShouldBe(renterId);
        result.Status.ShouldBe(OrderStatus.CREATED);
        result.PickUpDateTime.ShouldBeNull();
        result.PlannedReturnDate.ShouldBeNull();
        result.Note.ShouldBeNull();

        _mockRepository.Verify(r => r.FirstOrDefaultByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidOrderAndMultipleItems_RemovesItemsAndReturnsResponse()
    {
        // Arrange
        Setup();
        var orderId = Guid.NewGuid();
        var renterId = Guid.NewGuid();
        var order = MyLibrary.Domain.Order.Order.CreateEmpty(renterId);

        var ownerId = Guid.NewGuid();
        var itemId1 = Guid.NewGuid();
        order.AddItem(new OrderItem(itemId1, "item", ownerId));
        var itemId2 = Guid.NewGuid();
        order.AddItem(new OrderItem(itemId2, "item", ownerId));

        var itemIds = new List<Guid> { itemId1 };
        var command = new RemoveItemsFromOrderCommand(orderId, itemIds);

        _mockRepository.Setup(r => r.FirstOrDefaultByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(1);
        result.Items.ShouldNotContain(x => x.ItemId == itemId1);
        result.Items.ShouldContain(x => x.ItemId == itemId2);
        result.RenterId.ShouldBe(renterId);
        result.Status.ShouldBe(OrderStatus.CREATED);
        result.PickUpDateTime.ShouldBeNull();
        result.PlannedReturnDate.ShouldBeNull();
        result.Note.ShouldBeNull();

        _mockRepository.Verify(r => r.FirstOrDefaultByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        Setup();
        var orderId = Guid.NewGuid();

        var itemIds = new List<Guid> { Guid.NewGuid() };
        var command = new RemoveItemsFromOrderCommand(orderId, itemIds);

        _mockRepository.Setup(r => r.FirstOrDefaultByIdAsync(orderId, It.IsAny<CancellationToken>()))!
            .ReturnsAsync((MyLibrary.Domain.Order.Order?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None));

        exception.Message.ShouldBe($"Order with ID {orderId} not found.");
        _mockRepository.Verify(r => r.FirstOrDefaultByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}