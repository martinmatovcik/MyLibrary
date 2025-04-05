using MyLibrary.Domain.Abstraction.Entity;

namespace MyLibrary.Domain.Order.DomainEvents;

public record ItemRemovedFromOrder(Guid ItemId) : IDomainEvent;
//Handler should cancel reservation of item