using MyLibrary.Domain.Order.DomainEvents.Abstraction;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderPickedUp(Guid OrderId, Guid[] ItemIds, Guid RenterId) : OrderDomainEvent(OrderId);
//Handler should "rent()" each item