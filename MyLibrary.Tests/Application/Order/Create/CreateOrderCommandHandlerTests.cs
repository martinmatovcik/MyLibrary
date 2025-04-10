using Moq;
using MyLibrary.Application.Features.Order.Create;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Order;
using MyLibrary.Domain.Order.Repository;
using Shouldly;

namespace MyLibrary.Tests.Application.Order.Create;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateOrderCommandHandler _handler;
    
    public CreateOrderCommandHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateOrderCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);
    }
    
    private static void Setup() => new CreateOrderCommandHandlerTests();
    
    [Fact]
    public async Task Handle_ValidCommand_ReturnsOrderDetailResponse()
    {
        // Arrange
        Setup();
        
        var renterId = Guid.NewGuid();
        var command = new CreateOrderCommand(renterId);
        
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<MyLibrary.Domain.Order.Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

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
        
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<MyLibrary.Domain.Order.Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        Setup();
        
        var command = new CreateOrderCommand(Guid.NewGuid());
        
        var expectedException = new InvalidOperationException("Repository error");
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<MyLibrary.Domain.Order.Order>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(async () => await _handler.Handle(command, CancellationToken.None));

        exception.ShouldBe(expectedException);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<MyLibrary.Domain.Order.Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}