using MediatR;
using MyLibrary.Application.Features.Item.RentByIds;
using MyLibrary.Domain.Order.DomainEvents;

namespace MyLibrary.Application.Features.Item.Consumer;

sealed internal class OrderConfirmedConsumer(ISender sender) : INotificationHandler<OrderConfirmed>
{
    public async Task Handle(OrderConfirmed notification, CancellationToken cancellationToken)
    {
        await sender.Send(new RentByIdsCommand(notification.ItemIds, notification.RenterId, notification.PlannedReturnDate), cancellationToken);
    }
}