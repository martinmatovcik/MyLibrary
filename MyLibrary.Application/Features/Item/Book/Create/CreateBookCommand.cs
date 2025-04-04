using MediatR;
using MyLibrary.Application.Features.Item.Abstraction.Create;
using MyLibrary.Application.FUTURE_API.Item.Book.Create;

namespace MyLibrary.Application.Features.Item.Book.Create;

public record CreateBookCommand(string Name, string Author, int Year, string? Isbn, string? Description, Guid OwnerId)
    : CreateItemCommand<CreateBookResponse>(Name, Description, OwnerId);