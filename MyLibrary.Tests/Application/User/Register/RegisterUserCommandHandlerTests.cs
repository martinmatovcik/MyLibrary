using Moq;
using MyLibrary.Application.Features.User.Register;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.User;
using MyLibrary.Domain.User.Repository;
using Shouldly;

namespace MyLibrary.Tests.Application.User.Register;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new RegisterUserCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);
    }

    private static RegisterUserCommand NewCommand() =>
        new(
            "test@example.com",
            "testuser",
            "Password123!",
            "Test",
            "User",
            "1234567890");

    [Fact]
    public async Task Handle_ValidRegisterUserCommand_ReturnsRegisterUserResponse()
    {
        // Arrange
        new RegisterUserCommandHandlerTests();
        var command = NewCommand();

        _mockRepository.Setup(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Email.ShouldBe(command.Email);
        result.Username.ShouldBe(command.Username);

        _mockRepository.Verify(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailAlreadyInUse_ThrowsInvalidOperationException()
    {
        // Arrange
        new RegisterUserCommandHandlerTests();
        var command = NewCommand();

        _mockRepository.Setup(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None));

        exception.Message.ShouldBe("Email is already in use.");
        _mockRepository.Verify(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        new RegisterUserCommandHandlerTests();
        var command = NewCommand();

        var expectedException = new InvalidOperationException("Test exception");
        _mockRepository.Setup(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None));

        exception.ShouldBe(expectedException);
        _mockRepository.Verify(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UnitOfWorkThrowsException_PropagatesException()
    {
        // Arrange
        new RegisterUserCommandHandlerTests();
        var command = NewCommand();

        _mockRepository.Setup(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var expectedException = new InvalidOperationException("UnitOfWork exception");
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None));

        exception.ShouldBe(expectedException);
        _mockRepository.Verify(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}