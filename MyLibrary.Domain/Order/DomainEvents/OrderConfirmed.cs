using MyLibrary.Domain.Order.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderConfirmed(Guid OrderId) : OrderDomainEvent(OrderId);