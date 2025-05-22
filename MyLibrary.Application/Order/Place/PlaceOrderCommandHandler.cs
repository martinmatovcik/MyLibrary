using MediatR;
using MyLibrary.Application.Abstraction.Database;
using MyLibrary.Application.FUTURE_API.Order;
using MyLibrary.Application.Order.Mapper;
using MyLibrary.Application.Order.Repository;
using MyLibrary.Domain.Abstraction;

namespace MyLibrary.Application.Order.Place;

sealed internal class PlaceOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<PlaceOrderCommand, OrderDetailResponse>
{
    public async Task<OrderDetailResponse> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FirstOrDefaultByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new InvalidOperationException($"Order with ID {request.OrderId} not found.");
        
        order.Place(request.PickUpDateTime, request.PlannedReturnDate, request.Note);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return order.ToOrderDetailResponse();
    }
}