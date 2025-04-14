using MediatR;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Item.Abstraction.Repository;

namespace MyLibrary.Application.Features.Item.RentByIds;

sealed internal class RentByIdsCommandHandler(IItemRepository itemRepository, IUnitOfWork unitOfWork) : IRequestHandler<RentByIdsCommand>
{
    public async Task Handle(RentByIdsCommand request, CancellationToken cancellationToken)
    {
        var items = await itemRepository.GetByIdsAsync(request.ItemIds, cancellationToken);

        if (items.Length == 0)
            throw new InvalidOperationException("No items were found.");

        if (items.Length != request.ItemIds.Length)
        {
            var notFoundIds = request.ItemIds.Except(items.Select(x => x.Id)).ToArray();
            throw new InvalidOperationException($"Items with ids {notFoundIds} were not found.");
        }

        foreach (var item in items) 
            item.Rent(request.RenterId, request.PlannedReturnDate);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}