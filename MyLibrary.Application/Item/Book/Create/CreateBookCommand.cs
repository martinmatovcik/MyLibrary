using MyLibrary.Application.FUTURE_API.Item.Book.Create;
using MyLibrary.Application.Item.Abstraction.Create;

namespace MyLibrary.Application.Item.Book.Create;

public record CreateBookCommand(string Name, string Author, int Year, string? Isbn, string? Description, Guid OwnerId)
    : CreateItemCommand<CreateBookResponse>(Name, Description, OwnerId);