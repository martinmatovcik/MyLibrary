using MediatR;
using MyLibrary.Application.Abstraction.Database;
using MyLibrary.Application.FUTURE_API.User.Register;
using MyLibrary.Application.User.Repository;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.User;

namespace MyLibrary.Application.User.Register;

sealed internal class RegisterUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var isEmailAvailable = await userRepository.IsEmailAvailableAsync(request.Email, cancellationToken);
        
        if (!isEmailAvailable) throw new InvalidOperationException("Email is already in use."); //TODO-Feature: Result pattern   
        
        var newUser = LibraryUser.Create(new UserDetails(request.Email, request.Username, request.Password, request.FirstName, request.LastName, request.PhoneNumber));
        
        await userRepository.AddAsync(newUser, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new RegisterUserResponse(newUser.Id, newUser.Details.Email, newUser.Details.Username);
    }
}