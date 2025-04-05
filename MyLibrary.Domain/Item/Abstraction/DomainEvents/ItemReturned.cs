using MyLibrary.Domain.Item.Abstraction.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Item.Abstraction.DomainEvents;

public record ItemReturned(Guid ItemId, string Name) : ItemDomainEvent(ItemId, Name);