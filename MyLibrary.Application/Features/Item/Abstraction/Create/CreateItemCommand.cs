using MediatR;
using MyLibrary.Application.FUTURE_API.Item.Abstraction.Create;

namespace MyLibrary.Application.Features.Item.Abstraction.Create;

public abstract record CreateItemCommand<TResponse>(string Name, string? Description, Guid OwnerId) : IRequest<TResponse>
    where TResponse : CreateItemResponse;