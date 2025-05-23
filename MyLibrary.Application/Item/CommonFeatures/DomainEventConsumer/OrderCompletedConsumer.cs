using MediatR;
using MyLibrary.Application.Item.CommonFeatures.ReturnByIds;
using MyLibrary.Domain.Order.DomainEvents;

namespace MyLibrary.Application.Item.CommonFeatures.DomainEventConsumer;

sealed internal class OrderCompletedConsumer(ISender sender) : INotificationHandler<OrderCompleted>
{
    public async Task Handle(OrderCompleted notification, CancellationToken cancellationToken)
    {
        await sender.Send(new ReturnByIdsCommand(notification.ItemIds), cancellationToken);
    }
}