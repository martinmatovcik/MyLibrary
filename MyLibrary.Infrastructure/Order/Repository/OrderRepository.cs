using MyLibrary.Application.Order.Repository;
using MyLibrary.Infrastructure.Abstraction.Entity.Repository;
using MyLibrary.Infrastructure.Database;

namespace MyLibrary.Infrastructure.Order.Repository;

public class OrderRepository(MyLibraryDbContext dbContext) : EntityRepository<Domain.Order.Order>(dbContext), IOrderRepository
{
}