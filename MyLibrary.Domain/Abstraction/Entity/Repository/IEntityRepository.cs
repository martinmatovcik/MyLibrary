namespace MyLibrary.Domain.Abstraction.Entity.Repository;

public interface IEntityRepository<TEntity> where TEntity : Entity
{
    /// <summary>
    /// Adds an entity to the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Entity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an entity by its unique identifier asynchronously.
    /// </summary>
    /// <param name="entityId">The unique identifier of the entity to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation that returns the found entity.</returns>
    Task<TEntity> GetByIdAsync(Guid entityId, CancellationToken cancellationToken);
}