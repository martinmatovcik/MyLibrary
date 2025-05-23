using Microsoft.EntityFrameworkCore;
using MyLibrary.Domain.Abstraction.Entity.Repository;
using MyLibrary.Infrastructure.Database;

namespace MyLibrary.Infrastructure.Abstraction.Entity.Repository;

public class EntityRepository<TEntity>(MyLibraryDbContext dbContext) : IEntityRepository<TEntity> where TEntity : Domain.Abstraction.Entity.Entity
{
    protected readonly MyLibraryDbContext DbContext = dbContext;

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken) =>
        await DbContext.AddAsync(entity, cancellationToken);

    public async Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) => 
        await DbContext.AddRangeAsync(entities, cancellationToken);

    public async Task<TEntity?> FirstOrDefaultByIdAsync(Guid entityId, CancellationToken cancellationToken) =>
        await DbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == entityId, cancellationToken: cancellationToken);

    public async Task<TEntity[]> GetByIdsAsync(IEnumerable<Guid> entityIds, CancellationToken cancellationToken) =>
        await DbContext.Set<TEntity>().Where(x => entityIds.Contains(x.Id)).ToArrayAsync(cancellationToken);
}