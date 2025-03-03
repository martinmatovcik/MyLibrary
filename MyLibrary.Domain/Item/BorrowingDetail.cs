namespace MyLibrary.Domain.Item;

public abstract record BorrowingDetail(User.User Borrower, DateTime? BorrowedDate, DateTime? PlannedReturnDate, DateTime? RealReturnDate);