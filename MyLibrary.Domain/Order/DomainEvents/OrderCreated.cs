using MyLibrary.Domain.Order.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderCreated(Guid OrderId, Guid Renter) : OrderDomainEvent(OrderId);