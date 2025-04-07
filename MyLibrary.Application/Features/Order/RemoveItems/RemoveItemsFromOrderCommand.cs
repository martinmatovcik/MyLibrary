using MediatR;
using MyLibrary.Application.FUTURE_API.Order;

namespace MyLibrary.Application.Features.Order.RemoveItems;

public record RemoveItemsFromOrderCommand(Guid OrderId, List<Guid> ItemIds) : IRequest<OrderDetailResponse>;