namespace ECommerce.Modules.Ticketing.Infrastructure.Orders.Strategies;

internal sealed class FedexShippingStrategy : ShippingStrategyBase
{
    public override string ProviderName => "FedEx";
    protected override decimal BaseRate => 5.99m;
    protected override decimal PerItemRate => 1.50m;
    protected override decimal HeavyOrderSurcharge => 4.99m;
    protected override int HeavyOrderThreshold => 10;
    protected override decimal FreeShippingThreshold => 100m;
}
