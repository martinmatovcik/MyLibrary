namespace MyLibrary.Application.FUTURE_API.Item.Abstraction.Create;

public abstract record CreateItemResponse(Guid Id, string Name, string? Description, Guid OwnerId);