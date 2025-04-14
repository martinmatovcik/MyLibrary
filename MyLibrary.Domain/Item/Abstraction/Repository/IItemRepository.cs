using MyLibrary.Domain.Abstraction.Entity.Repository;

namespace MyLibrary.Domain.Item.Abstraction.Repository;

//TODO: Move to application
public interface IItemRepository
{
    Task<Item?> GetFirstByIdAsync(Guid id, CancellationToken cancellationToken);
}

public interface IItemRepository<TItem> : IEntityRepository<TItem> where TItem : Item
{
}