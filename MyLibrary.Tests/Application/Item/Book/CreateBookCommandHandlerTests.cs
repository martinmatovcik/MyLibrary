using Moq;
using MyLibrary.Application.Features.Item.Book.Create;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Item.Abstraction.Repository;
using Shouldly;

namespace MyLibrary.Tests.Application.Item.Book;

public class CreateBookCommandHandlerTests
{
    private readonly Mock<IItemRepository<MyLibrary.Domain.Item.Book.Book>> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateBookCommandHandler _handler;
    private readonly CreateBookCommand _command;

    public CreateBookCommandHandlerTests()
    {
        _mockRepository = new Mock<IItemRepository<MyLibrary.Domain.Item.Book.Book>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateBookCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);
        _command = new CreateBookCommand(
            "Test Book",
            "Author Name",
            2023,
            "978-3-16-148410-0",
            "Description",
            Guid.NewGuid());
    }

    private static void Setup() => new CreateBookCommandHandlerTests();

    [Fact]
    public async Task Handle_ValidCreateBookCommand_ReturnsCreateBookResponse()
    {
        // Arrange
        Setup();

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<MyLibrary.Domain.Item.Book.Book>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(_command.Name);
        result.Description.ShouldBe(_command.Description);
        result.OwnerId.ShouldBe(_command.OwnerId);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<MyLibrary.Domain.Item.Book.Book>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        Setup();

        var command = new CreateBookCommand(
            "Test Book",
            "Author Name",
            2023,
            "978-3-16-148410-0",
            "Description",
            Guid.NewGuid());

        var expectedException = new InvalidOperationException("Test exception");
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<MyLibrary.Domain.Item.Book.Book>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None));

        exception.ShouldBe(expectedException);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}