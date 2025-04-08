using Moq;
using MyLibrary.Application.Features.Item.Book.Create;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Item.Abstraction.Repository;
using Shouldly;

namespace MyLibrary.Tests.Application.Item.Book;

public class CreateBookCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCreateBookCommand_ReturnsCreateBookResponse()
    {
        // Arrange
        var mockRepository = new Mock<IItemRepository<MyLibrary.Domain.Item.Book.Book>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateBookCommandHandler(mockRepository.Object, mockUnitOfWork.Object);

        var ownerId = Guid.NewGuid();
        var command = new CreateBookCommand(
            "Test Book",
            "Author Name",
            2023,
            "978-3-16-148410-0",
            "Description",
            ownerId);

        mockRepository.Setup(r => r.AddAsync(It.IsAny<MyLibrary.Domain.Item.Book.Book>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(command.Name);
        result.Description.ShouldBe(command.Description);
        result.OwnerId.ShouldBe(command.OwnerId);

        mockRepository.Verify(r => r.AddAsync(It.IsAny<MyLibrary.Domain.Item.Book.Book>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var mockRepository = new Mock<IItemRepository<MyLibrary.Domain.Item.Book.Book>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateBookCommandHandler(mockRepository.Object, mockUnitOfWork.Object);
    
        var command = new CreateBookCommand(
            "Test Book",
            "Author Name",
            2023,
            "978-3-16-148410-0",
            "Description",
            Guid.NewGuid());
    
        var expectedException = new InvalidOperationException("Test exception");
        mockRepository.Setup(r => r.AddAsync(It.IsAny<MyLibrary.Domain.Item.Book.Book>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);
    
        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await handler.Handle(command, CancellationToken.None));
    
        exception.ShouldBe(expectedException);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}