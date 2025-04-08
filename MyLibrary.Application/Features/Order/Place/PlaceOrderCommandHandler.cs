using MediatR;
using MyLibrary.Application.Features.Order.Mapper;
using MyLibrary.Application.FUTURE_API.Order;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Order.Repository;

namespace MyLibrary.Application.Features.Order.Place;

sealed internal class PlaceOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<PlaceOrderCommand, OrderDetailResponse>
{
    public async Task<OrderDetailResponse> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new InvalidOperationException($"Order with ID {request.OrderId} not found.");
        
        order.Place(request.PickUpDateTime, request.PlannedReturnDate, request.Note);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return order.ToOrderDetailResponse();
    }
}