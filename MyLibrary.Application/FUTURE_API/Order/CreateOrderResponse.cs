using MyLibrary.Domain.Order;

namespace MyLibrary.Application.FUTURE_API.Order;

public record CreateOrderResponse(Guid Id, OrderStatus Status, Guid RenterId);