using MyLibrary.Domain.Abstraction.Entity.Repository;

namespace MyLibrary.Domain.User.Repository;

public interface IUserRepository : IEntityRepository<LibraryUser>
{
    Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken);
}