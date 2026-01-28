namespace ECommerce.Modules.Ticketing.Domain.Orders;

public enum OrderStatus
{
    Submitted = 0,
    AwaitingValidation = 1,
    StockConfirmed = 2,
    Paid = 3,
    Shipped = 4,
    Cancelled = 5,
}
