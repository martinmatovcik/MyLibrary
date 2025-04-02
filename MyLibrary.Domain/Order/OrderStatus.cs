namespace MyLibrary.Domain.Order;

public enum OrderStatus
{
    UNDEFINED = 0,
    CREATED,
    PENDING,
    PROCESSING,
    PLACED,
    CONFIRMED,
    CANCELED,
    AWAITING_PICKUP,
    // ITEMS_BORROWED,
    PICKED_UP,
    // ITEMS_RETURNED,
    COMPLETED,
    // REFUNDED,
    FAILED,
    ON_HOLD,
}