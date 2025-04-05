using MyLibrary.Domain.Helpers;
using NodaTime;

namespace MyLibrary.Domain.Abstraction.Entity;

public abstract class Entity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public EntityStatus EntityStatus { get; private set; } = EntityStatus.CREATED;
    
    public Instant Created { get; init; } = NodaTimeHelpers.NowInstant();
    
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    private bool IsStatus(EntityStatus status) => EntityStatus == status;

    private void SetStatus(EntityStatus newStatus) => EntityStatus = newStatus;

    public bool Equals(Entity? other) => other != null && Id == other.Id;
}