using MediatR;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Application.Abstraction.Database;
using MyLibrary.Application.Exceptions;
using MyLibrary.Domain.Abstraction.Entity;
using MyLibrary.Domain.Item.Abstraction;
using MyLibrary.Domain.Order;

namespace MyLibrary.Infrastructure;

public sealed class MyLibraryDbContext(DbContextOptions<MyLibraryDbContext> options, IPublisher publisher)
    : DbContext(options), IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyLibraryDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
    
    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            await PublishDomainEventsAsync(cancellationToken);

            return result;
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyException("Concurrency exception occurred.", exception);
        }
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<EntityStatus>().HaveConversion<string>();
        configurationBuilder.Properties<ItemStatus>().HaveConversion<string>();
        configurationBuilder.Properties<OrderStatus>().HaveConversion<string>();
        configurationBuilder.Properties<RentalDetailStatus>().HaveConversion<string>();
    }
}