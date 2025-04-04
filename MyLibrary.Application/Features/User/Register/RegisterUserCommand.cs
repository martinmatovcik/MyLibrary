using MediatR;
using MyLibrary.Application.FUTURE_API.User.Register;

namespace MyLibrary.Application.Features.User.Register;

public record RegisterUserCommand(string Email, string? Username, string Password, string FirstName, string LastName, string? PhoneNumber) : IRequest<RegisterUserResponse>;