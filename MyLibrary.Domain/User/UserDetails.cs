namespace MyLibrary.Domain.User;

public record UserDetails(
    string Username,
    string Password,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber)
{
    public string Username { get; private set; } = Username;
    public string Password { get; private set; } = Password;
    public string FirstName { get; private set; } = FirstName;
    public string LastName { get; private set; } = LastName;
    public string Email { get; private set; } = Email;
    public string PhoneNumber { get; private set; } = PhoneNumber;

    public static readonly UserDetails Empty = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
}