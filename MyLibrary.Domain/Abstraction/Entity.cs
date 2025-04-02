using MyLibrary.Domain.Helpers;
using NodaTime;

namespace MyLibrary.Domain.Abstraction;

public abstract class Entity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public EntityStatus EntityStatus { get; private set; } = EntityStatus.CREATED;
    
    public Instant Created { get; init; } = NodaTimeHelpers.NowInstant();
    
    public bool IsCreated() => IsStatus(EntityStatus.CREATED);

    public void SetCreated() => SetStatus(EntityStatus.CREATED);

    public bool IsDeleted() => IsStatus(EntityStatus.DELETED);

    public void SetDeleted() => SetStatus(EntityStatus.DELETED);
    
    private bool IsStatus(EntityStatus status) => EntityStatus == status;

    private void SetStatus(EntityStatus newStatus) => EntityStatus = newStatus;

    public bool Equals(Entity? other) => other != null && Id == other.Id;
}