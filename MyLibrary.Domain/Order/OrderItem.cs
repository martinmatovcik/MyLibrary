namespace MyLibrary.Domain.Order;

public record OrderItem
{
    public Guid ItemId { get; init; }
    public string Name { get; private set; } = string.Empty;
    public Guid Owner { get; private set; }

    public OrderItem(Guid itemId, string name, Guid owner)
    {
        ItemId = itemId;
        Name = name;
        Owner = owner;
    }
}