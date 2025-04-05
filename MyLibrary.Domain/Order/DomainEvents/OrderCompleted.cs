using MyLibrary.Domain.Abstraction.Entity;

namespace MyLibrary.Domain.Order.DomainEvents;

public record OrderCompleted(Guid[] ItemIds) : IDomainEvent;
// Handler should "return()" each item