using MyLibrary.Domain.Item.Abstraction;

namespace MyLibrary.Domain.Item.Book;

public class Book : Abstraction.Item
{
    public string Author { get; private set; } = string.Empty; //string might not be the best option
    public int Year { get; private set; } = 1990;
    public string? Isbn { get; private set; } // TODO: ISBN custom object with format checks
    
    private Book() // default constructor for EF Core
    {
    }
    
    private Book(string author, int year, string? isbn, string name, string? description, Guid owner, Guid? renter, List<RentalDetail> history, ItemStatus status)
        : base(name, description, owner, renter, history, status)
    {
        Author = author;
        Year = year;
        Isbn = isbn;
    }
    
    public static Book Create(string name, string author, int year, string? isbn, string? description, Guid owner) =>
        new(author, year, isbn, name, description, owner, null, [], ItemStatus.AVAILABLE);
}