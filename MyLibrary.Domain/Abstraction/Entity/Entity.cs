using MyLibrary.Domain.Helpers;
using NodaTime;

namespace MyLibrary.Domain.Abstraction.Entity;

public abstract class Entity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public EntityStatus EntityStatus { get; private set; } = EntityStatus.CREATED;
    
    public Instant Created { get; init; } = NodaTimeHelpers.NowInstant();
    
    private static readonly List<IDomainEvent> DomainEvents = [];

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => DomainEvents.ToList();

    public static void ClearDomainEvents() => DomainEvents.Clear();

    protected static void RaiseDomainEvent(IDomainEvent domainEvent) => DomainEvents.Add(domainEvent);

    protected bool IsStatus(EntityStatus status) => EntityStatus == status;

    protected void SetStatus(EntityStatus newStatus) => EntityStatus = newStatus;
}