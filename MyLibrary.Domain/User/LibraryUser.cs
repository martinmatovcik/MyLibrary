namespace MyLibrary.Domain.User;

public class LibraryUser : Abstraction.Entity
{
    public UserDetails Details { get; private set; } = UserDetails.Empty;
    public List<Item.Item> OwnedItems { get; private set; } = [];
    public List<Order.Order> Orders { get; private set; } = [];
}