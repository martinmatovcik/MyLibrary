namespace MyLibrary.Domain.Entity.Item;

public abstract class Item : Abstraction.Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public User.User Owner { get; private set; } = new();
    public Status Status { get; private set; } = Status.Available;
    public List<BorrowingDetails> BorrowingHistory { get; private set; } = [];

    public void SetName(string newName) => Name = newName;
    
    public void SetDescription(string newDescription) => SetDescriptionInternal(newDescription);

    public void RemoveDescription() => SetDescriptionInternal(null);

    private void SetDescriptionInternal(string? newDescription) => Description = newDescription;

    public void SetAvailable() => SetStatus(Status.Available);

    public void SetNotAvailable() => SetStatus(Status.NotAvailable);

    public void SetReserved() => SetStatus(Status.Reserved);
    
    private void SetStatus(Status newStatus) => Status = newStatus;

    //Borrow() - pozicanie
    //Return() - vratenie
    //Reserve() - Rezervacia na urcity cas (cas na rozhodnutie pred pozicanim)
    // GiveAway() - Darovenie itemu inemu userovi
}