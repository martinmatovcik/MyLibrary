using MyLibrary.Domain.Abstraction.Entity;

namespace MyLibrary.Domain.User;

public class LibraryUser : Entity //, ClaimsPrincipal????
{
    public UserDetails Details { get; private set; } = UserDetails.CreateEmpty();
    public List<UserItem> OwnedItems { get; private set; } = [];
    public List<UserOrder> Orders { get; private set; } = [];

    private LibraryUser()
    {
    }

    private LibraryUser(UserDetails userDetails, List<UserItem> ownedItems, List<UserOrder> orders)
    {
        Details = userDetails;
        OwnedItems = ownedItems;
        Orders = orders;
    }

    public static LibraryUser CreateEmpty() => new(UserDetails.CreateEmpty(), [], []);
    public static LibraryUser Create(UserDetails userDetails) => new(userDetails, [], []);
}