using MyLibrary.Application.FUTURE_API.Order;

namespace MyLibrary.Application.Order.Mapper;

static internal class OrderMapper
{
    static internal OrderDetailResponse ToOrderDetailResponse(this Domain.Order.Order order)
    {
        return new OrderDetailResponse(
            order.Id,
            order.Items.Select(x => new OrderItemDto(x.ItemId, x.Name, x.Owner)).ToList(),
            order.Renter,
            order.Status,
            order.PickUpDateTime,
            order.PlannedReturnDate,
            order.Note
            );
    }
}