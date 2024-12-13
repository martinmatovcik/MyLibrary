namespace MyLibrary.Domain.Entity.Item;

public class BorrowingDetails(Guid Borrower, DateTime? BorrowedDate, DateTime? PlannedReturnDate, DateTime? RealReturnDate) : Abstraction.Entity;