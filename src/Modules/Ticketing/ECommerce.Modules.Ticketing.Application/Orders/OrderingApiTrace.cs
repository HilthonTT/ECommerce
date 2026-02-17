using ECommerce.Modules.Ticketing.Domain.Orders;
using Microsoft.Extensions.Logging;

namespace ECommerce.Modules.Ticketing.Application.Orders;

internal static partial class OrderingApiTrace
{
    [LoggerMessage(EventId = 1, EventName = "OrderStatusUpdated", Level = LogLevel.Trace, Message = "Order with Id: {OrderId} has been successfully updated to status {Status}")]
    public static partial void LogOrderStatusUpdated(ILogger logger, Guid orderId, OrderStatus status);

    [LoggerMessage(EventId = 2, EventName = "PaymentMethodUpdated", Level = LogLevel.Trace, Message = "Order with Id: {OrderId} has been successfully updated with a payment method {PaymentMethod} ({Id})")]
    public static partial void LogOrderPaymentMethodUpdated(ILogger logger, Guid orderId, string paymentMethod, Guid id);

    [LoggerMessage(EventId = 3, EventName = "LogOrderCustomerAndPaymentValidatedOrUpdated", Level = LogLevel.Trace, Message = "Customer {CustomerId} and related payment method were validated or updated for order Id: {OrderId}.")]
    public static partial void LogOrderCustomerAndPaymentValidatedOrUpdated(ILogger logger, Guid customerId, Guid orderId);
}
