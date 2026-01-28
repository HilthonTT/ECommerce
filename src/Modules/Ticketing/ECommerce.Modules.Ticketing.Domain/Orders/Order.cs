using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;
using ECommerce.Modules.Ticketing.Domain.Customers;

namespace ECommerce.Modules.Ticketing.Domain.Orders;

[Auditable]
public sealed class Order : Entity
{
    private readonly List<OrderItem> _orderItems = [];

    public Guid Id { get; private init; }

    public Address Address { get; private set; } = default!;

    public Guid CustomerId { get; private init; }

    public OrderStatus Status { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public decimal TotalPrice { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private init; }

    private Order() { }

    public static Result<Order> Create(Guid customerId, Address address)
    {
        if (customerId == Guid.Empty)
        {
            return Result.Failure<Order>(OrderErrors.CustomerIdCannotBeEmpty);
        }

        if (address is null)
        {
            return Result.Failure<Order>(OrderErrors.AddressIsRequired);
        }

        var order = new Order
        {
            Id = Guid.CreateVersion7(),
            CustomerId = customerId,
            Address = address,
            Status = OrderStatus.Submitted,
            CreatedAtUtc = DateTime.UtcNow
        };

        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id));

        return Result.Success(order);
    }

    public static Result<Order> Create(Customer customer, Address address)
    {
        if (customer is null)
        {
            return Result.Failure<Order>(OrderErrors.CustomerCannotBeNull);
        }

        return Create(customer.Id, address);
    }

    public Result AddItem(
        string currency,
        Guid productId,
        string productName,
        decimal unitPrice,
        decimal discount,
        string pictureUrl,
        int units = 1)
    {
        if (Status != OrderStatus.Submitted)
        {
            return OrderErrors.CannotAddItemsToNonSubmittedOrder;
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            return OrderErrors.CurrencyIsRequired;
        }

        if (!string.IsNullOrEmpty(Currency) && Currency != currency)
        {
            return OrderErrors.CannotMixCurrencies(Currency);
        }

        OrderItem? existingOrderForProduct = _orderItems.SingleOrDefault(o => o.ProductId == productId);

        if (existingOrderForProduct is not null)
        {
            if (discount > existingOrderForProduct.Discount)
            {
                Result discountResult = existingOrderForProduct.SetNewDiscount(discount);
                if (discountResult.IsFailure)
                {
                    return discountResult;
                }
            }

            Result unitsResult = existingOrderForProduct.AddUnits(units);
            if (unitsResult.IsFailure)
            {
                return unitsResult;
            }
        }
        else
        {
            Result<OrderItem> orderItemResult = OrderItem.Create(
                productId,
                productName,
                unitPrice,
                discount,
                pictureUrl,
                units);

            if (orderItemResult.IsFailure)
            {
                return orderItemResult;
            }

            _orderItems.Add(orderItemResult.Value);
        }

        RecalculateTotalPrice();

        if (string.IsNullOrEmpty(Currency))
        {
            Currency = currency;
        }

        return Result.Success();
    }

    public Result RemoveItem(Guid productId)
    {
        if (Status != OrderStatus.Submitted)
        {
            return OrderErrors.CannotRemoveItemsFromNonSubmittedOrder;
        }

        OrderItem? orderItem = _orderItems.SingleOrDefault(o => o.ProductId == productId);
        if (orderItem is null)
        {
            return OrderErrors.OrderItemNotFound;
        }

        _orderItems.Remove(orderItem);
        RecalculateTotalPrice();

        return Result.Success();
    }

    public Result SetAwaitingValidationStatus()
    {
        if (Status != OrderStatus.Submitted)
        {
            return OrderErrors.CannotChangeStatusToAwaitingValidation(Status);
        }

        if (_orderItems.Count == 0)
        {
            return OrderErrors.CannotValidateOrderWithoutItems;
        }

        Status = OrderStatus.AwaitingValidation;
        RaiseDomainEvent(new OrderStatusChangedToAwaitingValidationDomainEvent(Id, _orderItems));

        return Result.Success();
    }

    public Result SetStockConfirmedStatus()
    {
        if (Status != OrderStatus.AwaitingValidation)
        {
            return OrderErrors.CannotChangeStatusToStockConfirmed(Status);
        }

        Status = OrderStatus.StockConfirmed;
        Description = "All the items were confirmed with available stock.";
        RaiseDomainEvent(new OrderStatusChangedToStockConfirmedDomainEvent(Id));

        return Result.Success();
    }

    public Result SetPaidStatus()
    {
        if (Status != OrderStatus.StockConfirmed)
        {
            return OrderErrors.CannotChangeStatusToPaid(Status);
        }

        Status = OrderStatus.Paid;
        Description = "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";
        RaiseDomainEvent(new OrderStatusChangedToPaidDomainEvent(Id, _orderItems));

        return Result.Success();
    }

    public Result SetShippedStatus()
    {
        if (Status != OrderStatus.Paid)
        {
            return OrderErrors.CannotShipOrder(Status);
        }

        Status = OrderStatus.Shipped;
        Description = "The order was shipped.";
        RaiseDomainEvent(new OrderShippedDomainEvent(Id));

        return Result.Success();
    }

    public Result SetCancelledStatus()
    {
        if (Status == OrderStatus.Paid || Status == OrderStatus.Shipped)
        {
            return OrderErrors.CannotCancelOrder(Status);
        }

        Status = OrderStatus.Cancelled;
        Description = "The order was cancelled.";
        RaiseDomainEvent(new OrderCancelledDomainEvent(Id));

        return Result.Success();
    }

    public Result SetCancelledStatusWhenStockIsRejected(IEnumerable<Guid> orderStockRejectedItems)
    {
        if (Status != OrderStatus.AwaitingValidation)
        {
            return OrderErrors.CannotCancelOrderDueToStockRejection(Status);
        }

        IEnumerable<string> itemsStockRejectedProductNames = OrderItems
            .Where(c => orderStockRejectedItems.Contains(c.ProductId))
            .Select(c => c.ProductName);

        string itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedProductNames);
        Description = $"The product items don't have stock: ({itemsStockRejectedDescription}).";

        Status = OrderStatus.Cancelled;

        return Result.Success();
    }

    private void RecalculateTotalPrice()
    {
        TotalPrice = _orderItems.Sum(o => o.UnitPrice * o.Units - o.Discount);
    }
}