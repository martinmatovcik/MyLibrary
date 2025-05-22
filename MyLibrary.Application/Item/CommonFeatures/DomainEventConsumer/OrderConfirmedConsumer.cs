using MediatR;
using MyLibrary.Application.Item.CommonFeatures.RentByIds;
using MyLibrary.Domain.Order.DomainEvents;

namespace MyLibrary.Application.Item.CommonFeatures.DomainEventConsumer;

sealed internal class OrderConfirmedConsumer(ISender sender) : INotificationHandler<OrderConfirmed>
{
    public async Task Handle(OrderConfirmed notification, CancellationToken cancellationToken)
    {
        await sender.Send(new RentByIdsCommand(notification.ItemIds, notification.RenterId, notification.PlannedReturnDate), cancellationToken);
    }
}