using MyLibrary.Domain.Order.DomainEvents.Abstraction;
using NodaTime;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderConfirmed(Guid OrderId, Guid[] ItemIds, Guid RenterId, LocalDate? PlannedReturnDate) : OrderDomainEvent(OrderId);