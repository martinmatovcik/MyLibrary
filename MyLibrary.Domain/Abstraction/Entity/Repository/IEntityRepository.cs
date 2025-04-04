namespace MyLibrary.Domain.Abstraction.Entity.Repository;

public interface IEntityRepository<TEntity> where TEntity : Entity
{
    Task AddAsync(Entity entity, CancellationToken cancellationToken);

    Task<TEntity> GetByIdAsync(Guid entityId, CancellationToken cancellationToken);
}