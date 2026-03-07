using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Domain.Products;

namespace ECommerce.Modules.Ticketing.Domain.Pricing;

public interface IPricingService
{
    Result<ProductPrice> CalculatePrice(Product product, int quantity, string? couponCode = null);

    Result<decimal> ApplyDiscount(decimal basePrice, decimal discountPercentage);
}
