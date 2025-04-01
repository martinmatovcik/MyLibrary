namespace MyLibrary.Domain.Order;

public enum OrderStatus
{
    UNDEFINED = 0,
    PENDING,
    CONFIRMED,
    PROCESSING,
    SHIPPED,
    DELIVERED,
    COMPLETED,
    RETURNED,
    REFUNDED,
    FAILED,
    ON_HOLD,
    AWAITING_PICKUP
}