using MediatR;
using MyLibrary.Application.FUTURE_API.Item.Abstraction.Create;

namespace MyLibrary.Application.Features.Item.Abstraction.Create;

/// <summary>
/// Abstract base command for creating items.
/// </summary>
/// <typeparam name="TResponse">The response type returned after item creation, must derive from <see cref="CreateItemResponse"/>.</typeparam>
/// <param name="Name">The name of the item to create.</param>
/// <param name="Description">Optional description of the item.</param>
/// <param name="OwnerId">The identifier of the user who will own this item.</param>
public abstract record CreateItemCommand<TResponse>(string Name, string? Description, Guid OwnerId) : IRequest<TResponse>
    where TResponse : CreateItemResponse;