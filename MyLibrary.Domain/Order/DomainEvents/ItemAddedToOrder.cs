using MyLibrary.Domain.Order.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Order.DomainEvents;

public record ItemAddedToOrder(Guid OrderId, Guid ItemId, Guid RenterId) : OrderDomainEvent(OrderId);
//Handler should reserve the item
