namespace ECommerce.Modules.Ticketing.Infrastructure.Orders.Strategies;

internal sealed class DhlShippingStrategy : ShippingStrategyBase
{
    public override string ProviderName => "DHL";
    protected override decimal BaseRate => 8.99m;
    protected override decimal PerItemRate => 1.75m;
    protected override decimal HeavyOrderSurcharge => 6.99m;
    protected override int HeavyOrderThreshold => 8;
    protected override decimal FreeShippingThreshold => 150m;
}
