using MyLibrary.Domain.Abstraction.Entity.Repository;

namespace MyLibrary.Domain.Item.Abstraction.Repository;

public interface IItemRepository<TItem> : IEntityRepository<TItem> where TItem : Item
{
}