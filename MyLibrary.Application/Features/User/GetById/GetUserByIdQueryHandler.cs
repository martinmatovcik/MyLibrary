using MediatR;
using MyLibrary.Domain.User;
using MyLibrary.Domain.User.Repository;

namespace MyLibrary.Application.Features.User.GetById;

sealed internal class GetUserByIdQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, LibraryUser>
{
    public async Task<LibraryUser> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultByIdAsync(request.Id, cancellationToken);

        if (user is null) throw new InvalidOperationException($"User with id {request.Id} was not found");
        
        return user;
    }
}