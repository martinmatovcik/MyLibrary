using MediatR;
using MyLibrary.Application.FUTURE_API.Item.Create;

namespace MyLibrary.Application.Features.Item.Create;

public record CreateItemCommand() : IRequest<CreateItemResponse>;