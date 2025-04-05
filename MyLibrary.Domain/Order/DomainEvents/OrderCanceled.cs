using MyLibrary.Domain.Abstraction.Entity;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderCanceled(Guid[] ItemIds) : IDomainEvent;
// Handler should "cancelReservation()" each item