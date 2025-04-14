using MediatR;

namespace MyLibrary.Application.Features.Item.ReserveById;

public record ReserveItemByIdCommand(Guid ItemId, Guid RenterId) : IRequest;