using MyLibrary.Domain.Abstraction.Entity.Repository;

namespace MyLibrary.Domain.Item.Abstraction.Repository;

//TODO: Move to application
public interface IItemRepository : IEntityRepository<Item>
{
}

public interface IItemRepository<TItem> : IEntityRepository<TItem> where TItem : Item
{
}