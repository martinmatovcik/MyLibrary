using MediatR;
using MyLibrary.Application.Features.Item.ReserveById;
using MyLibrary.Domain.Order.DomainEvents;

namespace MyLibrary.Application.Features.Item.Consumer;

sealed internal class ItemAddedToOrderConsumer(ISender sender) : INotificationHandler<ItemAddedToOrder>
{
    public async Task Handle(ItemAddedToOrder notification, CancellationToken cancellationToken)
    {
        await sender.Send(new ReserveItemByIdCommand(notification.ItemId, notification.RenterId), cancellationToken);
    }
}