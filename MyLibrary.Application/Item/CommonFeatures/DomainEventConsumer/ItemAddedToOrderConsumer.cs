using MediatR;
using MyLibrary.Application.Item.CommonFeatures.ReserveById;
using MyLibrary.Domain.Order.DomainEvents;

namespace MyLibrary.Application.Item.CommonFeatures.DomainEventConsumer;

sealed internal class ItemAddedToOrderConsumer(ISender sender) : INotificationHandler<ItemAddedToOrder>
{
    public async Task Handle(ItemAddedToOrder notification, CancellationToken cancellationToken)
    {
        await sender.Send(new ReserveByIdCommand(notification.ItemId, notification.RenterId), cancellationToken);
    }
}