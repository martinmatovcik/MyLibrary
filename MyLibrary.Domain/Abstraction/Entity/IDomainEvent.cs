using MediatR;

namespace MyLibrary.Domain.Abstraction.Entity;

public interface IDomainEvent : INotification; //TODO: Observer pattern to replace MediatR.INotification? https://medium.com/@lexitrainerph/observer-pattern-in-c-from-basics-to-advanced-ea4b2d748e 