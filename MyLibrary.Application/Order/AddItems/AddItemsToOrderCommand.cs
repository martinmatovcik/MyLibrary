using MediatR;
using MyLibrary.Application.FUTURE_API.Order;
using MyLibrary.Domain.Order;

namespace MyLibrary.Application.Order.AddItems;

public sealed record AddItemsToOrderCommand(Guid OrderId, List<OrderItem> OrderItems) : IRequest<OrderDetailResponse>;