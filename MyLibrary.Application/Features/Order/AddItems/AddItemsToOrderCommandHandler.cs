using MediatR;
using MyLibrary.Application.Features.Order.Mapper;
using MyLibrary.Application.FUTURE_API.Order;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Order.Repository;

namespace MyLibrary.Application.Features.Order.AddItems;

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