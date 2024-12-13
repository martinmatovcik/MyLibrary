namespace MyLibrary.Domain.Entity.User;

public class UserDetails(
    string username,
    string password,
    string firstName,
    string lastName,
    string email,
    string phoneNumber)
{
    public string Username { get; private set; } = username;
    public string Password { get; private set; } = password;
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public string Email { get; private set; } = email;
    public string PhoneNumber { get; private set; } = phoneNumber;

    public static readonly UserDetails Empty = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
}