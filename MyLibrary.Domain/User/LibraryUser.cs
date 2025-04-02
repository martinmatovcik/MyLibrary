namespace MyLibrary.Domain.User;

public class LibraryUser : Abstraction.Entity
{
    public UserDetails Details { get; private set; } = UserDetails.Empty;
    public List<Item.Item> OwnedItems { get; private set; } = [];
    public List<Order.Order> Orders { get; private set; } = [];

    private LibraryUser()
    {
    }

    private LibraryUser(UserDetails userDetails, List<Item.Item> ownedItems, List<Order.Order> orders)
    {
        Details = userDetails;
        OwnedItems = ownedItems;
        Orders = orders;
    }

    public static LibraryUser CreateEmpty() => new(UserDetails.Empty, [], []);
}