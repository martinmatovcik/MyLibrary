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
    private readonly RegisterUserCommand _command;

    public RegisterUserCommandHandlerTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new RegisterUserCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);
        _command = new RegisterUserCommand(
            "test@example.com",
            "testuser",
            "Password123!",
            "Test",
            "User",
            "1234567890");
    }
    
    private static void Setup() => new RegisterUserCommandHandlerTests();

    [Fact]
    public async Task Handle_ValidRegisterUserCommand_ReturnsRegisterUserResponse()
    {
        // Arrange
        Setup();

        _mockRepository.Setup(r => r.IsEmailAvailableAsync(_command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Email.ShouldBe(_command.Email);
        result.Username.ShouldBe(_command.Username);

        _mockRepository.Verify(r => r.IsEmailAvailableAsync(_command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailAlreadyInUse_ThrowsInvalidOperationException()
    {
        // Arrange
        Setup();

        _mockRepository.Setup(r => r.IsEmailAvailableAsync(_command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _handler.Handle(_command, CancellationToken.None));

        exception.Message.ShouldBe("Email is already in use.");
        _mockRepository.Verify(r => r.IsEmailAvailableAsync(_command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        Setup();

        var expectedException = new InvalidOperationException("Test exception");
        _mockRepository.Setup(r => r.IsEmailAvailableAsync(_command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _handler.Handle(_command, CancellationToken.None));

        exception.ShouldBe(expectedException);
        _mockRepository.Verify(r => r.IsEmailAvailableAsync(_command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UnitOfWorkThrowsException_PropagatesException()
    {
        // Arrange
        Setup();

        _mockRepository.Setup(r => r.IsEmailAvailableAsync(_command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var expectedException = new InvalidOperationException("UnitOfWork exception");
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _handler.Handle(_command, CancellationToken.None));

        exception.ShouldBe(expectedException);
        _mockRepository.Verify(r => r.IsEmailAvailableAsync(_command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}