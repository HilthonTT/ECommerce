using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public static class ProductBrandErrors
{
    public static readonly Error BrandNameIsRequired = Error.Problem(
        "ProductBrand.BrandNameIsRequired",
        "Brand name is required.");

    public static readonly Error BrandNameTooLong = Error.Problem(
        "ProductBrand.BrandNameTooLong",
        "Brand name cannot exceed 100 characters.");
}