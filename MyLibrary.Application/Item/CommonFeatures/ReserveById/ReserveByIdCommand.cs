using MediatR;

namespace MyLibrary.Application.Item.CommonFeatures.ReserveById;

public record ReserveByIdCommand(Guid ItemId, Guid RenterId) : IRequest;