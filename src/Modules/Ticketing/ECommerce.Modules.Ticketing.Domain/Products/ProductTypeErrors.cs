using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public static class ProductTypeErrors
{
    public static Error NotFound(Guid productTypeId) => Error.NotFound(
        "ProductType.NotFound",
        $"The product type Id = '{productTypeId}' was not found.");

    public static readonly Error TypeNameIsRequired = Error.Problem(
        "ProductType.TypeNameIsRequired",
        "Product type name is required.");

    public static readonly Error TypeNameTooLong = Error.Problem(
        "ProductType.TypeNameTooLong",
        "Product type name cannot exceed 100 characters.");
}