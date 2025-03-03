using MyLibrary.Domain.Abstraction;

namespace MyLibrary.Domain.Item;

public abstract class Item : StatusEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public User.User Owner { get; private set; } = new();
    public List<BorrowingDetail> History { get; private set; } = [];

    public void SetName(string newName) => Name = newName;

    public void SetDescription(string? newDescription) =>  Description = newDescription;

    public void RemoveDescription() => SetDescription(null);

    //Borrow() - pozicanie
    //Return() - vratenie
    //Reserve() - Rezervacia na urcity cas (cas na rozhodnutie pred pozicanim)
    // GiveAway() - Darovenie itemu inemu userovi
}