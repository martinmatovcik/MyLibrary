using MediatR;

namespace MyLibrary.Application.Features.Item.ReserveById;

public record ReserveByIdCommand(Guid ItemId, Guid RenterId) : IRequest;