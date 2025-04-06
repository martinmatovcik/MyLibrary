namespace MyLibrary.Domain.User;

public record UserItem
{
    public Guid ItemId { get; init; }
    public string Name { get; private set; } = string.Empty;
    public Guid Owner { get; private set; }

    public UserItem(Guid itemId, string name, Guid owner)
    {
        ItemId = itemId;
        Name = name;
        Owner = owner;
    }
}