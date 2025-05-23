using Microsoft.EntityFrameworkCore;
using MyLibrary.Application.User.Repository;
using MyLibrary.Domain.User;
using MyLibrary.Infrastructure.Abstraction.Entity.Repository;
using MyLibrary.Infrastructure.Database;

namespace MyLibrary.Infrastructure.User.Repository;

public class UserRepository(MyLibraryDbContext dbContext) : EntityRepository<LibraryUser>(dbContext), IUserRepository
{
    public async Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken) =>
        !await DbContext.Set<LibraryUser>().AnyAsync(x => x.Details.Email == email, cancellationToken: cancellationToken);
}