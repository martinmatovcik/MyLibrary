using MyLibrary.Domain.Abstraction;
using NodaTime;

namespace MyLibrary.Domain.Item;

public abstract class Item : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public User.LibraryUser Owner { get; private set; } = new();
    public List<RentalDetail> History { get; private set; } = [];
    public ItemStatus ItemStatus { get; private set; }
    
    public bool IsAvailable() => IsStatus(ItemStatus.AVAILABLE);
    
    public void SetAvailable() => SetStatus(ItemStatus.AVAILABLE);
    
    public void SetNotAvailable() => SetStatus(ItemStatus.NOT_AVAILABLE);

    // public bool IsReserved() => IsStatus(ItemStatus.RESERVED);
    //
    // public void SetReserved() => SetStatus(ItemStatus.RESERVED);

    private bool IsStatus(ItemStatus status) => ItemStatus == status;

    private void SetStatus(ItemStatus newStatus) => ItemStatus = newStatus;

    // public void SetName(string newName) => Name = newName;
    // public void SetDescription(string? newDescription) =>  Description = newDescription;
    // public void RemoveDescription() => SetDescription(null);

    public void Rent(User.LibraryUser renter, LocalDate? plannedReturnDate = null, string? note = null)
    {
        if (IsAvailable() && History.Exists(x => x.IsNotReturned()))
            throw new InvalidOperationException("Item cannot be rented because it has not been returned.");

        if (!IsAvailable())
            throw new InvalidOperationException("Item cannot be rented because it is not available.");

        SetNotAvailable();
        History.Add(RentalDetail.New(renter, plannedReturnDate, note, true));
    }

    public void Return(User.LibraryUser renter)
    {
        // var rentalDetail = History.Where(x => x.Renter == renter && x.is);
        //
        // if (IsAvailable() && History.Exists(x => x.IsReturned()))
        // {
        //     
        // }
        //
        // if (IsAvailable())
        //     throw new InvalidOperationException("Item cannot be returned because it is still available.");
        //
        // SetAvailable();
    }
    
    //Return() - vratenie
    //Reserve() - Rezervacia na urcity cas (cas na rozhodnutie pred pozicanim)
    // GiveAway() - Darovenie itemu inemu userovi
}