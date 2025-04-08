using MyLibrary.Domain.Order.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderCanceled(Guid OrderId, Guid[] ItemIds) : OrderDomainEvent(OrderId);
// Handler should "cancelReservation()" each item