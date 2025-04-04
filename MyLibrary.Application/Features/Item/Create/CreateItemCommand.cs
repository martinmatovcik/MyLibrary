using MediatR;
using MyLibrary.Application.FUTURE_API.Item.Book.Create;

namespace MyLibrary.Application.Features.Item.Create;

public record CreateItemCommand() : IRequest<CreateBookResponse>;