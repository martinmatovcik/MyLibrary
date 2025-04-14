using Moq;
using MyLibrary.Application.Features.Order.Place;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Helpers;
using MyLibrary.Domain.Order;
using MyLibrary.Domain.Order.Repository;
using Shouldly;

namespace MyLibrary.Tests.Application.Order.Place;

public class PlaceOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly PlaceOrderCommandHandler _handler;
    
    public PlaceOrderCommandHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new PlaceOrderCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);
    }
    
    private static void Setup() => new PlaceOrderCommandHandlerTests();
    
    [Fact]
    public async Task Handle_ValidOrder_PlacesOrderAndReturnsResponse()
    {
        // Arrange
        Setup();
        
        var orderId = Guid.NewGuid();
        var renterId = Guid.NewGuid();
        var order = MyLibrary.Domain.Order.Order.CreateEmpty(renterId);

        var itemId = Guid.NewGuid();
        order.AddItem(new OrderItem(itemId, "item", Guid.NewGuid()));

        var pickUpDateTime = NodaTimeHelpers.Now().PlusDays(1);
        var plannedReturnDate = NodaTimeHelpers.Today().PlusDays(7);
        var command = new PlaceOrderCommand(orderId, pickUpDateTime, plannedReturnDate, "Test order note");

        _mockRepository.Setup(r => r.FirstOrDefaultByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(1);
        result.RenterId.ShouldBe(renterId);
        result.Status.ShouldBe(OrderStatus.PLACED);
        result.PickUpDateTime.ShouldBe(pickUpDateTime);
        result.PlannedReturnDate.ShouldBe(plannedReturnDate);
        result.Note.ShouldNotBeEmpty();
        
        _mockRepository.Verify(r => r.FirstOrDefaultByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        Setup();
        
        var orderId = Guid.NewGuid();
        var order = MyLibrary.Domain.Order.Order.CreateEmpty(Guid.NewGuid());

        order.AddItem(new OrderItem(Guid.NewGuid(), "item", Guid.NewGuid()));

        var pickUpDateTime = NodaTimeHelpers.Now().PlusDays(1);
        var command = new PlaceOrderCommand(orderId, pickUpDateTime, null, null);

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