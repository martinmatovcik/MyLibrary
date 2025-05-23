// using MyLibrary.Domain.Helpers;
// using NodaTime;
//
// namespace MyLibrary.Domain.Item.Abstraction;
//
// public class RentalDetail(Instant created, Guid renter, LocalDate rentedDate, LocalDate? plannedReturnDate, LocalDateTime? realReturnDateTime, RentalDetailStatus status)
// {
//     public Instant Created { get; init; } = created;
//     public Guid Renter { get; init; } = renter;
//     public LocalDate RentedDate { get; init; } = rentedDate;
//     public LocalDate? PlannedReturnDate { get; init; } = plannedReturnDate;
//     public LocalDateTime? RealReturnDateTime { get; private set; } = realReturnDateTime;
//     public RentalDetailStatus Status { get; private set; } = status;
//     
//     public static RentalDetail CreateActive(Guid renter, LocalDate? plannedReturnDate = null) =>
//         new(NodaTimeHelpers.NowInstant(), renter, NodaTimeHelpers.Today(), plannedReturnDate, null, RentalDetailStatus.ACTIVE);
//
//     public void Return()
//     {
//         RealReturnDateTime = NodaTimeHelpers.Now();
//         Status = RentalDetailStatus.COMPLETED;
//     }
//     
//     internal bool IsReturned() => RealReturnDateTime is not null && Status == RentalDetailStatus.COMPLETED;
//     internal bool IsNotReturned() => !IsReturned();
// }

//TODO: History of item - add later