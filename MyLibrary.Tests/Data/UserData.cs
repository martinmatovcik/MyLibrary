using MyLibrary.Domain.User;

namespace MyLibrary.Tests.Data;

public class UserData
{
    public static LibraryUser CreateTestRenter() =>
        LibraryUser.CreateEmpty(new UserDetails("renter-name", "renter-password", "renter-first-name", "renter-last-name", "renter-email", "renter-phone"));
}