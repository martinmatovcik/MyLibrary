namespace MyLibrary.Domain.User;

public class LibraryUser : Abstraction.Entity
{
    public UserDetails Details { get; private set; } = UserDetails.CreateEmpty();
    public List<Item.Item> OwnedItems { get; private set; } = [];
    public List<Order.Order> Orders { get; private set; } = [];

    private LibraryUser()
    {
    }

    public LibraryUser(UserDetails userDetails, List<Item.Item> ownedItems, List<Order.Order> orders)
    {
        Details = userDetails;
        OwnedItems = ownedItems;
        Orders = orders;
    }

    public static LibraryUser CreateEmpty(UserDetails? userDetails = null) => new(userDetails ?? UserDetails.CreateEmpty(), [], []);
}