using MediatR;
using MyLibrary.Application.FUTURE_API.Order;
using MyLibrary.Domain.Order;

namespace MyLibrary.Application.Features.Order.AddItems;

public record AddItemsToOrderCommand(Guid OrderId, List<OrderItem> OrderItems) : IRequest<OrderDetailResponse>;