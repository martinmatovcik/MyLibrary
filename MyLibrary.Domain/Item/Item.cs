using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.User;
using NodaTime;

namespace MyLibrary.Domain.Item;

public abstract class Item : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public LibraryUser Owner { get; private set; } = LibraryUser.CreateEmpty();
    public LibraryUser? Renter { get; private set; }
    public List<RentalDetail> History { get; private set; } = []; //todo: toto treba prehodnotit
    public ItemStatus Status { get; private set; }

    public void Reserve(LibraryUser renter)
    {
        //TODO Feature: Rezervacia na obmedzeny cas
        
        if (IsStatus(ItemStatus.AVAILABLE) && History.Exists(x => x.IsNotReturned()))
            throw new InvalidOperationException("Item cannot be 'reserved' because it has not been returned.");

        if (!IsStatus(ItemStatus.AVAILABLE))
            throw new InvalidOperationException("Item cannot be 'reserved' because it is not available.");

        SetStatus(ItemStatus.RESERVED);
        SetRenter(renter);
    }

    public void CancelReservation()
    {
        if (!IsStatus(ItemStatus.RESERVED))
            throw new InvalidOperationException("Can not 'cancel reservation' because item is not reserved.");
        
        SetStatus(ItemStatus.AVAILABLE);
        SetRenter(null);
    }

    public void Rent(LibraryUser renter, LocalDate? plannedReturnDate)
    {
        if ((IsStatus(ItemStatus.AVAILABLE) || IsStatus(ItemStatus.RESERVED)) && History.Exists(x => x.IsNotReturned()))
            throw new InvalidOperationException("Item cannot be 'rented' because it has not been returned.");

        if (IsStatus(ItemStatus.NOT_AVAILABLE))
            throw new InvalidOperationException("Item cannot be 'rented' because it is not available.");

        if (IsStatus(ItemStatus.RESERVED) && Renter != renter)
            throw new InvalidOperationException("Item can not be 'rented' because it is 'reserved' by different user.");

        SetStatus(ItemStatus.NOT_AVAILABLE);
        SetRenter(renter);
        History.Add(RentalDetail.CreateActive(renter, plannedReturnDate));
    }

    public void Return()
    {
        if (!IsStatus(ItemStatus.NOT_AVAILABLE))
            throw new InvalidOperationException("Item can not be 'returned' because it has not been 'rented'.");
        
        SetStatus(ItemStatus.AVAILABLE);
        SetRenter(null);
        GetActiveHistoryItem().Return();
    }

    private RentalDetail GetActiveHistoryItem() => History.Single(x => x.Status == RentalDetailStatus.ACTIVE);
    
    private bool IsStatus(ItemStatus status) => Status == status;

    private void SetStatus(ItemStatus newStatus) => Status = newStatus;

    private void SetRenter(LibraryUser? renter) => Renter = renter;

    // GiveAway() - Darovenie itemu inemu userovi
}