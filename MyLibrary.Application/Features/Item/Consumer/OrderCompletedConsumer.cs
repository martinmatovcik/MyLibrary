using MediatR;
using MyLibrary.Application.Features.Item.ReturnByIds;
using MyLibrary.Domain.Order.DomainEvents;

namespace MyLibrary.Application.Features.Item.Consumer;

sealed internal class OrderCompletedConsumer(ISender sender) : INotificationHandler<OrderCompleted>
{
    public async Task Handle(OrderCompleted notification, CancellationToken cancellationToken)
    {
        await sender.Send(new ReturnByIdsCommand(notification.ItemIds), cancellationToken);
    }
}