using MyLibrary.Domain.Order;
using NodaTime;

namespace MyLibrary.Application.FUTURE_API.Order;

public record OrderDetailResponse(List<OrderItemDto> Items, Guid RenterId, OrderStatus Status, LocalDateTime? PickUpDateTime, LocalDate? PlannedReturnDate, string? Note);