using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Pricing;

public static class PricingErrors
{
    public static readonly Error ProductIsRequired =
        Error.Problem("Pricing.ProductIsRequired", "Product is required.");

    public static readonly Error ProductIsOutOfStock =
        Error.Problem("Pricing.ProductIsOutOfStock", "Product is out of stock.");

    public static readonly Error QuantityMustBePositive =
        Error.Problem("Pricing.QuantityMustBePositive", "Quantity must be greater than zero.");

    public static readonly Error BasePriceMustBePositive =
        Error.Problem("Pricing.BasePriceMustBePositive", "Base price must be greater than zero.");

    public static Error InsufficientStock(int available) =>
        Error.Problem("Pricing.InsufficientStock", $"Only {available} units are available.");

    public static Error InvalidDiscountPercentage(decimal max) =>
        Error.Problem("Pricing.InvalidDiscountPercentage",
            $"Discount percentage must be between 0 and {max}.");

    public static Error InvalidCouponCode(string code) =>
        Error.Problem("Pricing.InvalidCouponCode", $"Coupon code '{code}' is not valid.");
}