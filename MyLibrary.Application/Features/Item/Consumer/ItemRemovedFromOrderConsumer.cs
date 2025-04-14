using MediatR;
using MyLibrary.Application.Features.Item.CancelReservationById;
using MyLibrary.Domain.Order.DomainEvents;

namespace MyLibrary.Application.Features.Item.Consumer;

sealed internal class ItemRemovedFromOrderConsumer(ISender sender) : INotificationHandler<ItemRemovedFromOrder>
{
    public async Task Handle(ItemRemovedFromOrder notification, CancellationToken cancellationToken)
    {
        await sender.Send(new CancelReservationByIdCommand(notification.ItemId), cancellationToken);
    }
}