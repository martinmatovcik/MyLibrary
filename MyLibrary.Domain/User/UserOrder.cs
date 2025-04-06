namespace MyLibrary.Domain.User;

public record UserOrder
{
    public Guid OrderId { get; init; }
    public List<UserItem> Items { get; private set; }
    public Guid? ItemsOwner { get; private set; }
}