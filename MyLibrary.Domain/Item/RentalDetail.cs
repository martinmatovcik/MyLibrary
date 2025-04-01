using MyLibrary.Domain.Helpers;
using MyLibrary.Domain.User;
using NodaTime;

namespace MyLibrary.Domain.Item;

public class RentalDetail(Instant created, LibraryUser renter, LocalDate rentedDate, LocalDate? plannedReturnDate, LocalDateTime? realReturnDateTime, string? note, bool isCurrent)
{
    public Instant Created { get; init; } = created;
    public LibraryUser Renter { get; init; } = renter;
    public LocalDate RentedDate { get; init; } = rentedDate;
    public LocalDate? PlannedReturnDate { get; init; } = plannedReturnDate;
    public LocalDateTime? RealReturnDateTime { get; private set; } = realReturnDateTime;
    public string? Note { get; init; } = note;
    public bool IsCurrent { get; private set; } = isCurrent;
    
    internal static RentalDetail New(LibraryUser renter, LocalDate? plannedReturnDate = null, string? note = null, bool isCurrent = false) =>
        new(NodaTimeHelpers.NowInstant(), renter, NodaTimeHelpers.Today(), plannedReturnDate, null, note, isCurrent);

    internal void Return() => RealReturnDateTime = NodaTimeHelpers.Now();

    internal bool IsReturned() => RealReturnDateTime is not null;
    internal bool IsNotReturned() => !IsReturned();
}
