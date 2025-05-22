using Moq;
using MyLibrary.Application.Features.User.GetById;
using MyLibrary.Domain.User;
using MyLibrary.Domain.User.Repository;
using Shouldly;

namespace MyLibrary.Tests.Application.User.GetById;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly GetUserByIdQueryHandler _handler;
    
    public GetUserByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _handler = new GetUserByIdQueryHandler(_mockRepository.Object);
    }
    
    private static void Setup() => new GetUserByIdQueryHandlerTests();

    [Fact]
    public async Task Handle_UserExists_ReturnsUser()
    {
        // Arrange
        Setup();
        var userId = Guid.NewGuid();
        var expectedUser = LibraryUser.CreateEmpty();
        
        // Use reflection to set Id if it's a read-only property
        typeof(LibraryUser).GetProperty("Id")?.SetValue(expectedUser, userId);
        
        _mockRepository.Setup(r => r.FirstOrDefaultByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);
        
        // Act
        var result = await _handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);
        
        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedUser);
        _mockRepository.Verify(r => r.FirstOrDefaultByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_UserDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        Setup();
        var userId = Guid.NewGuid();
        
        _mockRepository.Setup(r => r.FirstOrDefaultByIdAsync(userId, It.IsAny<CancellationToken>()))!
            .ReturnsAsync((LibraryUser?)null);
        
        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None));
        
        exception.Message.ShouldBe($"User with id {userId} was not found");
        _mockRepository.Verify(r => r.FirstOrDefaultByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        Setup();
        var userId = Guid.NewGuid();
        var expectedException = new Exception("Database connection error");
        
        _mockRepository.Setup(r => r.FirstOrDefaultByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);
        
        // Act & Assert
        var exception = await Should.ThrowAsync<Exception>(
            async () => await _handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None));
        
        exception.ShouldBe(expectedException);
        _mockRepository.Verify(r => r.FirstOrDefaultByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}