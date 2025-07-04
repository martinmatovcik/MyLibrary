using MediatR;
using MyLibrary.Application.FUTURE_API.Order;
using NodaTime;

namespace MyLibrary.Application.Order.Place;

public record PlaceOrderCommand(Guid OrderId, LocalDateTime PickUpDateTime, LocalDate? PlannedReturnDate, string? Note) : IRequest<OrderDetailResponse>;