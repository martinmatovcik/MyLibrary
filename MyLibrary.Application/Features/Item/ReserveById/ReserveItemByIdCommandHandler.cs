using MediatR;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Item.Abstraction.Repository;

namespace MyLibrary.Application.Features.Item.ReserveById;

sealed internal class ReserveItemByIdCommandHandler(IItemRepository itemRepository, IUnitOfWork unitOfWork) : IRequestHandler<ReserveItemByIdCommand>
{
    public async Task Handle(ReserveItemByIdCommand request, CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetFirstByIdAsync(request.ItemId, cancellationToken);

        if (item is null)
            throw new InvalidOperationException($"Item with id {request.ItemId} not found.");
        
        item.Reserve(request.RenterId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}