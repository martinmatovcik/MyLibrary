using MediatR;
using MyLibrary.Domain.User;

namespace MyLibrary.Application.Features.User.GetById;

public record GetUserByIdQuery(Guid Id) : IRequest<LibraryUser>;