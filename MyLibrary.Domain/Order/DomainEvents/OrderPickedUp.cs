using MyLibrary.Domain.Abstraction.Entity;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderPickedUp(Guid[] ItemIds) : IDomainEvent;
//Handler should "rent()" each item