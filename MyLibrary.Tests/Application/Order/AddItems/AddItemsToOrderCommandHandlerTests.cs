using Moq;
using MyLibrary.Application.Abstraction.Database;
using MyLibrary.Application.Order.AddItems;
using MyLibrary.Application.Order.Repository;
using MyLibrary.Domain.Order;
using Shouldly;

namespace MyLibrary.Tests.Application.Order.AddItems;

public class AddItemsToOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly AddItemsToOrderCommandHandler _handler;
    
    public AddItemsToOrderCommandHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new AddItemsToOrderCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);
    }
    
    private static void Setup() => new AddItemsToOrderCommandHandlerTests();
    
    [Fact]
    public async Task Handle_ValidOrderAndItems_AddsItemsAndReturnsResponse()
    {
        // Arrange
        Setup();
        var orderId = Guid.NewGuid();
        
        var ownerId = Guid.NewGuid();
        var renterId = Guid.NewGuid();
        var order = MyLibrary.Domain.Order.Order.CreateEmpty(renterId);
        
        var orderItems = new List<OrderItem> { new(renterId, "item", ownerId) };
        var command = new AddItemsToOrderCommand(orderId, orderItems);
        
        _mockRepository.Setup(r => r.FirstOrDefaultByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
            
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(1);
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
        
        var ownerId = Guid.NewGuid();
        
        var orderItems = new List<OrderItem> { new(Guid.NewGuid(), "item", ownerId) };
        var command = new AddItemsToOrderCommand(orderId, orderItems);
        
        _mockRepository.Setup(r => r.FirstOrDefaultByIdAsync(orderId, It.IsAny<CancellationToken>()))!
            .ReturnsAsync((MyLibrary.Domain.Order.Order?)null);
            
        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None));
            
        exception.Message.ShouldBe($"Order with ID {orderId} not found.");
        _mockRepository.Verify(r => r.FirstOrDefaultByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_WithMultipleItems_AddsAllItemsToOrder()
    {
        // Arrange
        Setup();
        var orderId = Guid.NewGuid();
        
        var ownerId = Guid.NewGuid();
        var renterId = Guid.NewGuid();
        var order = MyLibrary.Domain.Order.Order.CreateEmpty(renterId);
        
        var orderItems = new List<OrderItem> { new(Guid.NewGuid(), "item", ownerId), new(Guid.NewGuid(), "item2", ownerId) };
        var command = new AddItemsToOrderCommand(orderId, orderItems);
        
        _mockRepository.Setup(r => r.FirstOrDefaultByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
            
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(2);
        result.RenterId.ShouldBe(renterId);
        result.Status.ShouldBe(OrderStatus.CREATED);
        result.PickUpDateTime.ShouldBeNull();
        result.PlannedReturnDate.ShouldBeNull();
        result.Note.ShouldBeNull();
        
        _mockRepository.Verify(r => r.FirstOrDefaultByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}