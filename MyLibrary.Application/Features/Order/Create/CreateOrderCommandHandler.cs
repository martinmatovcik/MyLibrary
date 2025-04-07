using MediatR;
using MyLibrary.Application.Features.Order.Mapper;
using MyLibrary.Application.FUTURE_API.Order;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Order.Repository;

namespace MyLibrary.Application.Features.Order.Create;

public class CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, OrderDetailResponse>
{
    public async Task<OrderDetailResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Domain.Order.Order.CreateEmpty(request.RenterId);
        
        await orderRepository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return order.ToOrderDetailResponse();
    }
}