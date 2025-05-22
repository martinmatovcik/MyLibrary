using MediatR;
using NodaTime;

namespace MyLibrary.Application.Item.CommonFeatures.RentByIds;

public record RentByIdsCommand(Guid[] ItemIds, Guid RenterId, LocalDate? PlannedReturnDate) : IRequest;