using MyLibrary.Domain.User;

namespace MyLibrary.Tests.Data;

public class UserData
{
    public static LibraryUser CreateTestRenter() =>
        LibraryUser.CreateEmpty();
}