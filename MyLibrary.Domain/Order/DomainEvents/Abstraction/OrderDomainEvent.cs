using MyLibrary.Domain.Abstraction.Entity;

namespace MyLibrary.Domain.Order.DomainEvents.Abstraction;

public abstract record OrderDomainEvent(Guid OrderId) : IDomainEvent;