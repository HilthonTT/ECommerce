using ECommerce.Modules.Ticketing.Domain.Orders;

namespace ECommerce.Modules.Ticketing.Application.Abstractions.Orders;

public interface IShippingStrategy
{
    string ProviderName { get; }

    decimal CalculateCost(Order order);
}
