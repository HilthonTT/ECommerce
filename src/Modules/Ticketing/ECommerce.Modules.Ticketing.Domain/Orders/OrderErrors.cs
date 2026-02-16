using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Orders;

public static class OrderErrors
{
    public static Error NotFound(Guid orderId) => Error.Problem(
        "Order.NotFound",
        $"Order with ID {orderId} was not found.");

    public static readonly Error CustomerIdCannotBeEmpty = Error.Problem(
        "Order.CustomerIdCannotBeEmpty",
        "Customer ID cannot be empty.");

    public static readonly Error CustomerCannotBeNull = Error.Problem(
        "Order.CustomerCannotBeNull",
        "Customer cannot be null.");

    public static readonly Error AddressIsRequired = Error.Problem(
        "Order.AddressIsRequired",
        "Address is required.");

    public static readonly Error CannotAddItemsToNonSubmittedOrder = Error.Problem(
        "Order.CannotAddItemsToNonSubmittedOrder",
        "Cannot add items to an order that is not in Submitted status.");

    public static readonly Error CurrencyIsRequired = Error.Problem(
        "Order.CurrencyIsRequired",
        "Currency is required.");

    public static Error CannotMixCurrencies(string orderCurrency) => Error.Problem(
        "Order.CannotMixCurrencies",
        $"Cannot mix currencies. Order currency is {orderCurrency}.");

    public static readonly Error CannotRemoveItemsFromNonSubmittedOrder = Error.Problem(
        "Order.CannotRemoveItemsFromNonSubmittedOrder",
        "Cannot remove items from an order that is not in Submitted status.");

    public static readonly Error OrderItemNotFound = Error.Problem(
        "Order.OrderItemNotFound",
        "Order item not found.");

    public static Error CannotChangeStatusToAwaitingValidation(OrderStatus currentStatus) => Error.Problem(
        "Order.CannotChangeStatusToAwaitingValidation",
        $"Cannot change status from {currentStatus} to AwaitingValidation.");

    public static readonly Error CannotValidateOrderWithoutItems = Error.Problem(
        "Order.CannotValidateOrderWithoutItems",
        "Cannot validate an order without items.");

    public static Error CannotChangeStatusToStockConfirmed(OrderStatus currentStatus) => Error.Problem(
        "Order.CannotChangeStatusToStockConfirmed",
        $"Cannot change status from {currentStatus} to StockConfirmed.");

    public static Error CannotChangeStatusToPaid(OrderStatus currentStatus) => Error.Problem(
        "Order.CannotChangeStatusToPaid",
        $"Cannot change status from {currentStatus} to Paid.");

    public static Error CannotShipOrder(OrderStatus currentStatus) => Error.Problem(
        "Order.CannotShipOrder",
        $"Cannot ship an order with status {currentStatus}. Order must be Paid.");

    public static Error CannotCancelOrder(OrderStatus currentStatus) => Error.Problem(
        "Order.CannotCancelOrder",
        $"Cannot cancel an order with status {currentStatus}.");

    public static Error CannotCancelOrderDueToStockRejection(OrderStatus currentStatus) => Error.Problem(
        "Order.CannotCancelOrderDueToStockRejection",
        $"Cannot cancel order due to stock rejection. Current status is {currentStatus}.");

    public static readonly Error InvalidDiscount = Error.Problem(
        "Order.InvalidDiscount",
        "The discount is invalid.");

    public static readonly Error InvalidUnits = Error.Problem(
        "Order.InvalidUnits",
        "The units value is invalid.");

    public static readonly Error TotalOrderItemLowerThanAppliedDiscount = Error.Problem(
        "Order.TotalOrderItemLowerThanAppliedDiscount",
        "The total of order item is lower than applied discount.");
}
