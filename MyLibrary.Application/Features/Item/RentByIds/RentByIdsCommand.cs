using MediatR;
using NodaTime;

namespace MyLibrary.Application.Features.Item.RentByIds;

public record RentByIdsCommand(Guid[] ItemIds, Guid RenterId, LocalDate? PlannedReturnDate) : IRequest;