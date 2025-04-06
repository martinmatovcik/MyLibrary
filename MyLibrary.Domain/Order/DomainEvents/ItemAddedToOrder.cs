using MyLibrary.Domain.Abstraction.Entity;

namespace MyLibrary.Domain.Order.DomainEvents;

public record ItemAddedToOrder(Guid ItemId, Guid RenterId) : IDomainEvent;
//Handler should reserve the item
