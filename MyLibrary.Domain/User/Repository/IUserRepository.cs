using MyLibrary.Domain.Abstraction.Entity.Repository;

namespace MyLibrary.Domain.User.Repository;

public interface IUserRepository : IEntityRepository
{
    Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken);
}