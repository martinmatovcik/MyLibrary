namespace MyLibrary.Domain.User;

public record UserDetails(
    string Email,
    string? Username,
    string Password, //TODO-Feature: Hash password
    string FirstName,
    string LastName,
    string? PhoneNumber)
{
    public string Email { get; private set; } = Email;
    public string? Username { get; private set; } = Username;
    public string Password { get; private set; } = Password;
    public string FirstName { get; private set; } = FirstName;
    public string LastName { get; private set; } = LastName;
    public string? PhoneNumber { get; private set; } = PhoneNumber;

    public static UserDetails CreateEmpty() => new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
}