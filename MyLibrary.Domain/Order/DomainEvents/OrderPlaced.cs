using MyLibrary.Domain.Order.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderPlaced(Guid OrderId) : OrderDomainEvent(OrderId);