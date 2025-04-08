using MyLibrary.Domain.Order.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Order.DomainEvents;

public record ItemRemovedFromOrder(Guid OrderId, Guid ItemId) : OrderDomainEvent(OrderId);
//Handler should cancel reservation of item