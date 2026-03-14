namespace ECommerce.Modules.Ticketing.Infrastructure.Orders.Strategies;

internal sealed class UspsShippingStrategy : ShippingStrategyBase
{
    public override string ProviderName => "USPS";
    protected override decimal BaseRate => 4.49m;
    protected override decimal PerItemRate => 0.99m;
    protected override decimal HeavyOrderSurcharge => 3.49m;
    protected override int HeavyOrderThreshold => 12;
    protected override decimal FreeShippingThreshold => 75m;
}
