using MediatR;
using MyLibrary.Application.FUTURE_API.Order;

namespace MyLibrary.Application.Features.Order.Create;

public record CreateOrderCommand(Guid RenterId) : IRequest<CreateOrderResponse>;