using MyLibrary.Domain.Abstraction.Entity;

namespace MyLibrary.Domain.Item.Abstraction.DomainEvents.Abstraction;

public abstract record ItemDomainEvent(Guid ItemId, string Name) : IDomainEvent;