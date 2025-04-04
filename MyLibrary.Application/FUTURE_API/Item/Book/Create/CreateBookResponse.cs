using MyLibrary.Application.FUTURE_API.Item.Abstraction.Create;

namespace MyLibrary.Application.FUTURE_API.Item.Book.Create;

public record CreateBookResponse(Guid Id, string Name, string? Description, Guid OwnerId) : CreateItemResponse(Id, Name, Description, OwnerId);