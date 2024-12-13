namespace MyLibrary.Domain.Entity.Abstraction;

public abstract class Entity
{
    public Guid Id { get; init; } = Guid.NewGuid();
}