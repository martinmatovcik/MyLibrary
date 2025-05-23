using MediatR;
using MyLibrary.Application.FUTURE_API.Order;

namespace MyLibrary.Application.Order.RemoveItems;

public record RemoveItemsFromOrderCommand(Guid OrderId, List<Guid> ItemIds) : IRequest<OrderDetailResponse>;