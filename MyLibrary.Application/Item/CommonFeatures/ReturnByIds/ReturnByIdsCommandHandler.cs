using MediatR;
using MyLibrary.Application.Abstraction.Database;
using MyLibrary.Application.Item.Repository;

namespace MyLibrary.Application.Item.CommonFeatures.ReturnByIds;

sealed internal class ReturnByIdsCommandHandler(IItemRepository itemRepository, IUnitOfWork unitOfWork) : IRequestHandler<ReturnByIdsCommand>
{
    public async Task Handle(ReturnByIdsCommand request, CancellationToken cancellationToken)
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
            item.Return();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}