using MyLibrary.Domain.Order.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderAwaitingPickup(Guid OrderId) : OrderDomainEvent(OrderId);