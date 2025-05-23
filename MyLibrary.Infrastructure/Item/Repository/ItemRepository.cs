using MyLibrary.Application.Item.Repository;
using MyLibrary.Infrastructure.Abstraction.Entity.Repository;
using MyLibrary.Infrastructure.Database;

namespace MyLibrary.Infrastructure.Item.Repository;

public class ItemRepository(MyLibraryDbContext dbContext) : EntityRepository<Domain.Item.Abstraction.Item>(dbContext), IItemRepository
{
}