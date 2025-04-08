using Moq;
using MyLibrary.Application.Features.User.Register;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.User;
using MyLibrary.Domain.User.Repository;
using Shouldly;

namespace MyLibrary.Tests.Application.User.Register;

public class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRegisterUserCommand_ReturnsRegisterUserResponse()
    {
        // Arrange
        var mockRepository = new Mock<IUserRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var handler = new RegisterUserCommandHandler(mockRepository.Object, mockUnitOfWork.Object);

        var userId = Guid.NewGuid();
        var command = new RegisterUserCommand(
            "test@example.com",
            "testuser",
            "Password123!",
            "Test",
            "User",
            "1234567890");

        mockRepository.Setup(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        mockRepository.Setup(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Email.ShouldBe(command.Email);
        result.Username.ShouldBe(command.Username);

        mockRepository.Verify(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        mockRepository.Verify(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailAlreadyInUse_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockRepository = new Mock<IUserRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var handler = new RegisterUserCommandHandler(mockRepository.Object, mockUnitOfWork.Object);

        var command = new RegisterUserCommand(
            "existing@example.com",
            "testuser",
            "Password123!",
            "Test",
            "User",
            "1234567890");

        mockRepository.Setup(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await handler.Handle(command, CancellationToken.None));

        exception.Message.ShouldBe("Email is already in use.");
        mockRepository.Verify(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        mockRepository.Verify(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var mockRepository = new Mock<IUserRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var handler = new RegisterUserCommandHandler(mockRepository.Object, mockUnitOfWork.Object);

        var command = new RegisterUserCommand(
            "test@example.com",
            "testuser",
            "Password123!",
            "Test",
            "User",
            "1234567890");

        var expectedException = new InvalidOperationException("Test exception");
        mockRepository.Setup(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        mockRepository.Setup(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await handler.Handle(command, CancellationToken.None));

        exception.ShouldBe(expectedException);
        mockRepository.Verify(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        mockRepository.Verify(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UnitOfWorkThrowsException_PropagatesException()
    {
        // Arrange
        var mockRepository = new Mock<IUserRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var handler = new RegisterUserCommandHandler(mockRepository.Object, mockUnitOfWork.Object);

        var command = new RegisterUserCommand(
            "test@example.com",
            "testuser",
            "Password123!",
            "Test",
            "User",
            "1234567890");

        mockRepository.Setup(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var expectedException = new InvalidOperationException("UnitOfWork exception");
        mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await handler.Handle(command, CancellationToken.None));

        exception.ShouldBe(expectedException);
        mockRepository.Verify(r => r.IsEmailAvailableAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        mockRepository.Verify(r => r.AddAsync(It.IsAny<LibraryUser>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}