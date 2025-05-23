using MediatR;
using MyLibrary.Application.Item.CommonFeatures.CancelReservationById;
using MyLibrary.Domain.Order.DomainEvents;

namespace MyLibrary.Application.Item.CommonFeatures.DomainEventConsumer;

sealed internal class ItemRemovedFromOrderConsumer(ISender sender) : INotificationHandler<ItemRemovedFromOrder>
{
    public async Task Handle(ItemRemovedFromOrder notification, CancellationToken cancellationToken)
    {
        await sender.Send(new CancelReservationByIdCommand(notification.ItemId), cancellationToken);
    }
}