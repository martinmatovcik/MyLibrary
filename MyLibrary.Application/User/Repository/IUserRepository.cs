using MyLibrary.Domain.Abstraction.Entity.Repository;
using MyLibrary.Domain.User;

namespace MyLibrary.Application.User.Repository;

public interface IUserRepository : IEntityRepository<LibraryUser>
{
    Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken);
}