using MyLibrary.Domain.Order.DomainEvents.Abstraction;
using NodaTime;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderPlaced(Guid OrderId, LocalDateTime pickUpDateTime) : OrderDomainEvent(OrderId);