using MediatR;
using MyLibrary.Application.User.Repository;
using MyLibrary.Domain.User;

namespace MyLibrary.Application.User.GetById;

sealed internal class GetUserByIdQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, LibraryUser>
{
    public async Task<LibraryUser> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultByIdAsync(request.Id, cancellationToken);

        if (user is null) throw new InvalidOperationException($"User with id {request.Id} was not found");
        
        return user;
    }
}