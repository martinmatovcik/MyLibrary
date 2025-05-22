using MyLibrary.Domain.Abstraction.Entity.Repository;

namespace MyLibrary.Application.Order.Repository;

public interface IOrderRepository : IEntityRepository<Domain.Order.Order>;