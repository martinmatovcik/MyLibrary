using MediatR;

namespace MyLibrary.Application.Item.CommonFeatures.CancelReservationById;

public record CancelReservationByIdCommand(Guid ItemId) : IRequest;