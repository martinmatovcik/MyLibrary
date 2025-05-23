using MyLibrary.Domain.Abstraction.Entity;
using MyLibrary.Domain.Item.Abstraction.DomainEvents;

namespace MyLibrary.Domain.Item.Abstraction;

public abstract class Item : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid Owner { get; private set; } = Guid.Empty;
    public Guid? Renter { get; private set; }
    // public List<RentalDetail> History { get; private set; } = [];
    public ItemStatus Status { get; private set; }

    protected Item()
    {
    }
    
    protected Item(string name, string? description, Guid owner, Guid? renter, ItemStatus status)
    {
        Name = name;
        Description = description;
        Owner = owner;
        Renter = renter;
        Status = status;
    }

    public void RaiseCreatedEvent() => RaiseDomainEvent(new ItemCreated(Id, Name, Owner));

    public void Reserve(Guid renter)
    {
        //TODO Feature: Rezervacia na obmedzeny cas
        if (!IsStatus(ItemStatus.AVAILABLE))
            throw new InvalidOperationException("Item cannot be 'reserved' because it is not available.");

        SetStatus(ItemStatus.RESERVED);
        SetRenter(renter);
        
        RaiseDomainEvent(new ItemReserved(Id, Name, Renter!.Value));
    }

    public void CancelReservation()
    {
        if (!IsStatus(ItemStatus.RESERVED))
            throw new InvalidOperationException("Can not 'cancel reservation' because item is not reserved.");
        
        SetStatus(ItemStatus.AVAILABLE);
        SetRenter(null);
        
        RaiseDomainEvent(new ItemReservationCanceled(Id, Name));
    }

    public void Rent(Guid renter)
    {
        if (IsStatus(ItemStatus.NOT_AVAILABLE))
            throw new InvalidOperationException("Item cannot be 'rented' because it is not available.");

        if (IsStatus(ItemStatus.RESERVED) && Renter != renter)
            throw new InvalidOperationException("Item can not be 'rented' because it is 'reserved' by different user.");

        SetStatus(ItemStatus.NOT_AVAILABLE);
        SetRenter(renter);
        
        RaiseDomainEvent(new ItemRented(Id, Name, Renter!.Value));
    }

    public void Return()
    {
        if (!IsStatus(ItemStatus.NOT_AVAILABLE))
            throw new InvalidOperationException("Item can not be 'returned' because it has not been 'rented'.");
        
        SetStatus(ItemStatus.AVAILABLE);
        SetRenter(null);
        
        RaiseDomainEvent(new ItemReturned(Id, Name));
    }
    
    private bool IsStatus(ItemStatus status) => Status == status;

    private void SetStatus(ItemStatus newStatus) => Status = newStatus;

    private void SetRenter(Guid? renter) => Renter = renter;

    // GiveAway() - Darovenie itemu inemu userovi
}