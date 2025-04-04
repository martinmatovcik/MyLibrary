using MediatR;
using MyLibrary.Application.Features.User.GetById;
using MyLibrary.Application.FUTURE_API.Order;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Order.Repository;

namespace MyLibrary.Application.Features.Order.Create;

public class CreateOrderCommandHandler(ISender sender, IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var renter = await sender.Send(new GetUserByIdQuery(request.RenterId), cancellationToken);
        
        var order = Domain.Order.Order.CreateEmpty(renter);
        
        await orderRepository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new CreateOrderResponse(order.Id, order.Status, renter.Id);
    }
}