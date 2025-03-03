namespace MyLibrary.Domain.Abstraction;

public abstract class Entity
{
    public Guid Id { get; init; } = Guid.NewGuid();
}