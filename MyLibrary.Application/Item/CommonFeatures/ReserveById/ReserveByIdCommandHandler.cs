using MediatR;
using MyLibrary.Application.Abstraction.Database;
using MyLibrary.Application.Item.Repository;
using MyLibrary.Domain.Abstraction;

namespace MyLibrary.Application.Item.CommonFeatures.ReserveById;

sealed internal class ReserveByIdCommandHandler(IItemRepository itemRepository, IUnitOfWork unitOfWork) : IRequestHandler<ReserveByIdCommand>
{
    public async Task Handle(ReserveByIdCommand request, CancellationToken cancellationToken)
    {
        var item = await itemRepository.FirstOrDefaultByIdAsync(request.ItemId, cancellationToken);

        if (item is null)
            throw new InvalidOperationException($"Item with id {request.ItemId} not found.");
        
        item.Reserve(request.RenterId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}