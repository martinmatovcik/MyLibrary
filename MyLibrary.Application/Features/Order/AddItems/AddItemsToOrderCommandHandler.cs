using MediatR;
using MyLibrary.Application.FUTURE_API.Order;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Order.Repository;

namespace MyLibrary.Application.Features.Order.AddItems;

public class AddItemsToOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<AddItemsToOrderCommand, OrderDetailResponse>
{
    public async Task<OrderDetailResponse> Handle(AddItemsToOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new InvalidOperationException($"Order with ID {request.OrderId} not found.");

        foreach (var item in request.OrderItems)
            order.AddItem(item);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new OrderDetailResponse(
            order.Items.Select(x => new OrderItemDto(x.ItemId, x.Name, x.Owner)).ToList(),
            order.Renter,
            order.Status,
            order.PickUpDateTime,
            order.PlannedReturnDate,
            order.Note);
    }
}