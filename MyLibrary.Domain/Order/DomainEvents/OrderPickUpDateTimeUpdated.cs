using MyLibrary.Domain.Order.DomainEvents.Abstraction;
using NodaTime;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderPickUpDateTimeUpdated(Guid OrderId, LocalDateTime PickUpDateTime) : OrderDomainEvent(OrderId);