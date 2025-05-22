using MediatR;

namespace MyLibrary.Application.Item.CommonFeatures.ReturnByIds;

public record ReturnByIdsCommand(Guid[] ItemIds) : IRequest;