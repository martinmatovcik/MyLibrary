using MyLibrary.Application.FUTURE_API.Order;

namespace MyLibrary.Application.Features.Order.Mapper;

public static class OrderMapper
{
    public static OrderDetailResponse ToOrderDetailResponse(this Domain.Order.Order order)
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