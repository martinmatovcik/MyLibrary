using MediatR;

namespace MyLibrary.Application.Features.Item.CancelReservationById;

public record CancelReservationByIdCommand(Guid ItemId) : IRequest;