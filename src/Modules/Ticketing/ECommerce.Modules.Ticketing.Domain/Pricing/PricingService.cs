using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Domain.Products;

namespace ECommerce.Modules.Ticketing.Domain.Pricing;

public sealed class PricingService : IPricingService
{
    private const decimal MaxDiscount = 100;

    public Result<decimal> ApplyDiscount(decimal basePrice, decimal discountPercentage)
    {
        if (basePrice < 0)
        {
            return PricingErrors.BasePriceMustBePositive;
        }

        if (discountPercentage is < 0 or > MaxDiscount)
        {
            return PricingErrors.InvalidDiscountPercentage(MaxDiscount);
        }

        decimal discountAmount = basePrice * (discountPercentage / 100m);
        return Math.Round(basePrice - discountAmount, 2);
    }

    public Result<ProductPrice> CalculatePrice(Product product, int quantity, string? couponCode = null)
    {
        if (quantity <= 0)
        {
            return PricingErrors.QuantityMustBePositive;
        }

        decimal discount = CalculateBulkDiscount(quantity);

        if (!string.IsNullOrWhiteSpace(couponCode))
        {
            var couponResult = ApplyCouponDiscount(couponCode, discount);
            if (couponResult.IsFailure)
            {
                return couponResult.Error;
            }
            discount = couponResult.Value;
        }

        Result<decimal> discountedPriceResult = ApplyDiscount(product.Price, discount);
        if (discountedPriceResult.IsFailure)
        {
            return discountedPriceResult.Error; 
        }

        decimal discountedUnitPrice = discountedPriceResult.Value;
        decimal totalDiscount = Math.Round((product.Price - discountedUnitPrice) * quantity, 2);
        decimal totalPrice = Math.Round(discountedUnitPrice * quantity, 2);

        return new ProductPrice(
          product.Id,
          product.Price,
          discountedUnitPrice,
          discount,
          quantity,
          totalDiscount,
          totalPrice);
    }

    private static decimal CalculateBulkDiscount(int quantity) => quantity switch
    {
        >= 50 => 15m,
        >= 20 => 10m,
        >= 10 => 5m,
        >= 5 => 2.5m,
        _ => 0m
    };

    private static Result<decimal> ApplyCouponDiscount(string couponCode, decimal existingDiscount)
    {
        decimal couponDiscount = couponCode.ToUpperInvariant() switch
        {
            "SAVE10" => 10m,
            "SAVE20" => 20m,
            _ => -1,
        };

        if (couponDiscount < 0)
        {
            return Result.Failure<decimal>(PricingErrors.InvalidCouponCode(couponCode));
        }

        return Math.Min(existingDiscount + couponDiscount, 80m);
    }
}
