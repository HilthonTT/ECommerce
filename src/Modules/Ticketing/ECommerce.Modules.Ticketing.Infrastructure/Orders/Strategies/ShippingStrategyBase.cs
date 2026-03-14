using ECommerce.Modules.Ticketing.Application.Abstractions.Orders;
using ECommerce.Modules.Ticketing.Domain.Orders;

namespace ECommerce.Modules.Ticketing.Infrastructure.Orders.Strategies;

internal abstract class ShippingStrategyBase : IShippingStrategy
{
    public abstract string ProviderName { get; }

    protected abstract decimal BaseRate { get; }
    protected abstract decimal PerItemRate { get; }
    protected abstract decimal HeavyOrderSurcharge { get; }
    protected abstract int HeavyOrderThreshold { get; }
    protected abstract decimal FreeShippingThreshold { get; }

    public decimal CalculateCost(Order order)
    {
        if (order.TotalPrice >= FreeShippingThreshold)
        {
            return 0m;
        }

        int totalUnits = order.OrderItems.Sum(oi => oi.Units);

        decimal cost = BaseRate + (totalUnits * PerItemRate);

        if (totalUnits >= HeavyOrderThreshold)
        {
            cost += HeavyOrderSurcharge;
        }

        return Math.Round(cost, 2);
    }
}
