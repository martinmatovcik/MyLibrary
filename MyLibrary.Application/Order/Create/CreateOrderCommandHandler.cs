using MediatR;
using MyLibrary.Application.Abstraction.Database;
using MyLibrary.Application.FUTURE_API.Order;
using MyLibrary.Application.Order.Mapper;
using MyLibrary.Application.Order.Repository;
using MyLibrary.Domain.Abstraction;

namespace MyLibrary.Application.Order.Create;

sealed internal class CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, OrderDetailResponse>
{
    public async Task<OrderDetailResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Domain.Order.Order.CreateEmpty(request.RenterId);
        
        await orderRepository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return order.ToOrderDetailResponse();
    }
}