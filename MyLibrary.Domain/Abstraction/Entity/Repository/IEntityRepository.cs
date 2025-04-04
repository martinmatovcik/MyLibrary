namespace MyLibrary.Domain.Abstraction.Entity.Repository;

public interface IEntityRepository
{
    Task AddAsync(Entity entity, CancellationToken cancellationToken);
}