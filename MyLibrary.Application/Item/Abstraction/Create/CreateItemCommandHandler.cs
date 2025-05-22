using MediatR;
using MyLibrary.Application.Abstraction.Database;
using MyLibrary.Application.FUTURE_API.Item.Abstraction.Create;
using MyLibrary.Application.Item.Repository;
using MyLibrary.Domain.Abstraction;

namespace MyLibrary.Application.Item.Abstraction.Create;

/// <summary>
/// Abstract base handler for creating item entities.
/// Handles the creation process including validation, persistence, and response generation.
/// </summary>
/// <typeparam name="TItem">The specific domain item type to create, must derive from <see cref="Domain.Item.Abstraction.Item"/>.</typeparam>
/// <typeparam name="TCommand">The command containing creation details, must implement <see cref="CreateItemCommand{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The response type returned after item creation, must implement <see cref="CreateItemResponse"/>.</typeparam>
/// <param name="itemRepository">Repository for persisting the created item.</param>
/// <param name="unitOfWork">Unit of work to commit the transaction.</param>
abstract internal class CreateItemCommandHandler<TItem, TCommand, TResponse>(IItemRepository<TItem> itemRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<TCommand, TResponse> where TItem : Domain.Item.Abstraction.Item where TCommand : CreateItemCommand<TResponse> where TResponse : CreateItemResponse
{
    public async Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken)
    {
        var item = CreateItem(request, request.OwnerId);
        item.RaiseCreatedEvent();
        
        await itemRepository.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    
        return CreateResponse(item);
    }

    /// <summary>
    /// Creates a domain item entity from the command request and owner.
    /// </summary>
    /// <param name="request">The command request containing item details.</param>
    /// <param name="owner">The library user who will own the item.</param>
    /// <returns>A new domain item entity.</returns>
    protected abstract TItem CreateItem(TCommand request, Guid owner);
    
    /// <summary>
    /// Creates a response DTO from the domain item entity.
    /// </summary>
    /// <param name="item">The domain item entity.</param>
    /// <returns>A response DTO containing item details.</returns>
    protected abstract TResponse CreateResponse(TItem item);
}