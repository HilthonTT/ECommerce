namespace ECommerce.Modules.Ticketing.Infrastructure.Orders.Strategies;

internal sealed class UpsShippingStrategy : ShippingStrategyBase
{
    public override string ProviderName => "UPS";
    protected override decimal BaseRate => 7.49m;
    protected override decimal PerItemRate => 1.25m;
    protected override decimal HeavyOrderSurcharge => 5.99m;
    protected override int HeavyOrderThreshold => 8;
    protected override decimal FreeShippingThreshold => 120m;
}
