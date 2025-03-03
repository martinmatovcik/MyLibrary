namespace MyLibrary.Domain.User;

public class User : Abstraction.Entity
{
    public UserDetails Details { get; private set; } = UserDetails.Empty;
    public List<Item.Item> OwnedItems { get; private set; } = [];
    public List<Item.Item> BorrowedItems { get; private set; } = [];
}