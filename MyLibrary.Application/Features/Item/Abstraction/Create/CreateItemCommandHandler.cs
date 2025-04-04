using MediatR;
using MyLibrary.Application.Features.User.GetById;
using MyLibrary.Application.FUTURE_API.Item.Abstraction.Create;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Item.Abstraction.Repository;

namespace MyLibrary.Application.Features.Item.Abstraction.Create;

public abstract class CreateItemCommandHandler<TItem, TCommand, TResponse>(ISender sender, IItemRepository<TItem> itemRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<TCommand, TResponse> where TItem : Domain.Item.Abstraction.Item where TCommand : CreateItemCommand<TResponse> where TResponse : CreateItemResponse
{
    public async Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken)
    {
        //TODO-feature: should be done using separate http-call? Learn modular monoliths...
        var owner = await sender.Send(new GetUserByIdQuery(request.OwnerId), cancellationToken);
        var item = CreateItem(request, owner);
        
        await itemRepository.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    
        return CreateResponse(item);
    }

    protected abstract TItem CreateItem(TCommand request, Domain.User.LibraryUser owner);
    protected abstract TResponse CreateResponse(TItem item);
}