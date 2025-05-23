using MyLibrary.Application.User.Repository;
using MyLibrary.Domain.User;
using MyLibrary.Infrastructure.Abstraction.Entity.Repository;
using MyLibrary.Infrastructure.Database;

namespace MyLibrary.Infrastructure.User.Repository;

public class UserRepository(MyLibraryDbContext dbContext) : EntityRepository<LibraryUser>(dbContext), IUserRepository
{
    public Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken) => throw new NotImplementedException();
}