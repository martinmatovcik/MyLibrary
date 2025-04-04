using MediatR;
using MyLibrary.Application.FUTURE_API.User.Register;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.User;
using MyLibrary.Domain.User.Repository;

namespace MyLibrary.Application.Features.User.Register;

public class RegisterUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
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