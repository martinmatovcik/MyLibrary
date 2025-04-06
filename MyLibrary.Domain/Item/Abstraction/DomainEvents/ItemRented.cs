using MyLibrary.Domain.Item.Abstraction.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Item.Abstraction.DomainEvents;

public record ItemRented(Guid ItemId, string Name, Guid Renter) : ItemDomainEvent(ItemId, Name);