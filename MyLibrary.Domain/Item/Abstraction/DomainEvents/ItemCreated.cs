using MyLibrary.Domain.Item.Abstraction.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Item.Abstraction.DomainEvents;

public record ItemCreated(Guid ItemId, string Name, Guid Owner) : ItemDomainEvent(ItemId, Name);