using MediatR;
using MyLibrary.Application.Abstraction.Database;
using MyLibrary.Application.FUTURE_API.Order;
using MyLibrary.Application.Order.Mapper;
using MyLibrary.Application.Order.Repository;

namespace MyLibrary.Application.Order.RemoveItems;

sealed internal class RemoveItemsFromOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<RemoveItemsFromOrderCommand, OrderDetailResponse>
{
    public async Task<OrderDetailResponse> Handle(RemoveItemsFromOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FirstOrDefaultByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new InvalidOperationException($"Order with ID {request.OrderId} not found.");

        foreach (var itemId in request.ItemIds)
            order.RemoveItem(itemId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return order.ToOrderDetailResponse();
    }
}