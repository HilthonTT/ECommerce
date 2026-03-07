namespace ECommerce.Modules.Ticketing.Domain.Pricing;

public sealed record ProductPrice(
    int ProductId,
    decimal OriginalUnitPrice,
    decimal DiscountedUnitPrice,
    decimal DiscountPercentage,
    int Quantity,
    decimal TotalDiscount,
    decimal TotalPrice)
{
    public bool HasDiscount => DiscountPercentage > 0;
}