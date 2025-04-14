using MediatR;

namespace MyLibrary.Application.Features.Item.ReturnByIds;

public record ReturnByIdsCommand(Guid[] ItemIds) : IRequest;