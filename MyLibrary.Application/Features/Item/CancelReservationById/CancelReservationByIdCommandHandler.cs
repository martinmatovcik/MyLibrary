using MediatR;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Item.Abstraction.Repository;

namespace MyLibrary.Application.Features.Item.CancelReservationById;

sealed internal class CancelReservationByIdCommandHandler(IItemRepository itemRepository, IUnitOfWork unitOfWork) : IRequestHandler<CancelReservationByIdCommand>
{
    public async Task Handle(CancelReservationByIdCommand request, CancellationToken cancellationToken)
    {
        var item = await itemRepository.FirstOrDefaultByIdAsync(request.ItemId, cancellationToken);
        
        if (item is null)
            throw new InvalidOperationException($"Item with id {request.ItemId} not found.");
        
        item.CancelReservation();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}