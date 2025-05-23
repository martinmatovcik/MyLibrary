using MediatR;
using MyLibrary.Application.Abstraction.Database;
using MyLibrary.Application.FUTURE_API.Order;
using MyLibrary.Application.Order.Mapper;
using MyLibrary.Application.Order.Repository;

namespace MyLibrary.Application.Order.AddItems;

sealed internal class AddItemsToOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<AddItemsToOrderCommand, OrderDetailResponse>
{
    public async Task<OrderDetailResponse> Handle(AddItemsToOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FirstOrDefaultByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new InvalidOperationException($"Order with ID {request.OrderId} not found.");

        foreach (var item in request.OrderItems)
            order.AddItem(item);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return order.ToOrderDetailResponse();
    }
}