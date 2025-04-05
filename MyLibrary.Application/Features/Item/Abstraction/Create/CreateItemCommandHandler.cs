using MediatR;
using MyLibrary.Application.Features.User.GetById;
using MyLibrary.Application.FUTURE_API.Item.Abstraction.Create;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Item.Abstraction.Repository;

namespace MyLibrary.Application.Features.Item.Abstraction.Create;

/// <summary>
/// Abstract base handler for creating item entities.
/// Handles the creation process including validation, persistence, and response generation.
/// </summary>
/// <typeparam name="TItem">The specific domain item type to create, must derive from <see cref="Domain.Item.Abstraction.Item"/>.</typeparam>
/// <typeparam name="TCommand">The command containing creation details, must implement <see cref="CreateItemCommand{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The response type returned after item creation, must implement <see cref="CreateItemResponse"/>.</typeparam>
/// <param name="sender">Mediator to send nested queries like retrieving the owner.</param>
/// <param name="itemRepository">Repository for persisting the created item.</param>
/// <param name="unitOfWork">Unit of work to commit the transaction.</param>
public abstract class CreateItemCommandHandler<TItem, TCommand, TResponse>(ISender sender, IItemRepository<TItem> itemRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<TCommand, TResponse> where TItem : Domain.Item.Abstraction.Item where TCommand : CreateItemCommand<TResponse> where TResponse : CreateItemResponse
{
    public async Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken)
    {
        //TODO-feature: should be done using separate http-call / message bus event? Learn modular monoliths...
        var owner = await sender.Send(new GetUserByIdQuery(request.OwnerId), cancellationToken);
        var item = CreateItem(request, owner);
        
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
    protected abstract TItem CreateItem(TCommand request, Domain.User.LibraryUser owner);
    
    /// <summary>
    /// Creates a response DTO from the domain item entity.
    /// </summary>
    /// <param name="item">The domain item entity.</param>
    /// <returns>A response DTO containing item details.</returns>
    protected abstract TResponse CreateResponse(TItem item);
}