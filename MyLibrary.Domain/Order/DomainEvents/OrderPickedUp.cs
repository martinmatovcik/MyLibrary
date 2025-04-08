using MyLibrary.Domain.Order.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderPickedUp(Guid OrderId, Guid[] ItemIds) : OrderDomainEvent(OrderId);
//Handler should "rent()" each item