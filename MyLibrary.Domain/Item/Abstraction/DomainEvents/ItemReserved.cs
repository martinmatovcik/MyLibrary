using MyLibrary.Domain.Item.Abstraction.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Item.Abstraction.DomainEvents;

public record ItemReserved(Guid ItemId, string Name, Guid Renter) : ItemDomainEvent(ItemId, Name);