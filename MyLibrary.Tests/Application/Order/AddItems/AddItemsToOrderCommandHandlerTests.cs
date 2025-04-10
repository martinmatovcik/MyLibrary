using Moq;
using MyLibrary.Application.Features.Order.AddItems;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Order;
using MyLibrary.Domain.Order.Repository;
using Shouldly;

namespace MyLibrary.Tests.Application.Order.AddItems;

public class AddItemsToOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidOrderAndItems_AddsItemsAndReturnsResponse()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var mockRepository = new Mock<IOrderRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var handler = new AddItemsToOrderCommandHandler(mockRepository.Object, mockUnitOfWork.Object);
        
        var ownerId = Guid.NewGuid();
        var renterId = Guid.NewGuid();
        var order = MyLibrary.Domain.Order.Order.CreateEmpty(renterId);
        
        var orderItems = new List<OrderItem> { new(renterId, "item", ownerId) };
        var command = new AddItemsToOrderCommand(orderId, orderItems);
        
        mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
            
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(1);
        result.RenterId.ShouldBe(renterId);
        result.Status.ShouldBe(OrderStatus.CREATED);
        result.PickUpDateTime.ShouldBeNull();
        result.PlannedReturnDate.ShouldBeNull();
        result.Note.ShouldBeNull();
        
        mockRepository.Verify(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_OrderNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var mockRepository = new Mock<IOrderRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var handler = new AddItemsToOrderCommandHandler(mockRepository.Object, mockUnitOfWork.Object);
        
        var ownerId = Guid.NewGuid();
        
        var orderItems = new List<OrderItem> { new(Guid.NewGuid(), "item", ownerId) };
        var command = new AddItemsToOrderCommand(orderId, orderItems);
        
        mockRepository.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MyLibrary.Domain.Order.Order)null);
            
        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await handler.Handle(command, CancellationToken.None));
            
        exception.Message.ShouldBe($"Order with ID {orderId} not found.");
        mockRepository.Verify(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_WithMultipleItems_AddsAllItemsToOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var mockRepository = new Mock<IOrderRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var handler = new AddItemsToOrderCommandHandler(mockRepository.Object, mockUnitOfWork.Object);
        
        var ownerId = Guid.NewGuid();
        var renterId = Guid.NewGuid();
        var order = MyLibrary.Domain.Order.Order.CreateEmpty(renterId);
        
        var orderItems = new List<OrderItem> { new(Guid.NewGuid(), "item", ownerId), new(Guid.NewGuid(), "item2", ownerId) };
        var command = new AddItemsToOrderCommand(orderId, orderItems);
        
        mockRepository.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
            
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Count.ShouldBe(2);
        result.RenterId.ShouldBe(renterId);
        result.Status.ShouldBe(OrderStatus.CREATED);
        result.PickUpDateTime.ShouldBeNull();
        result.PlannedReturnDate.ShouldBeNull();
        result.Note.ShouldBeNull();
        
        mockRepository.Verify(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}