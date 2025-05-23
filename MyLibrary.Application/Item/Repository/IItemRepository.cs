using MyLibrary.Domain.Abstraction.Entity.Repository;

namespace MyLibrary.Application.Item.Repository;

public interface IItemRepository : IEntityRepository<Domain.Item.Abstraction.Item>
{
}

public interface IItemRepository<TItem> : IEntityRepository<TItem> where TItem : Domain.Item.Abstraction.Item
{
}