using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public static class CatalogTypeErrors
{
    public static readonly Error TypeNameIsRequired = Error.Problem(
        "CatalogType.TypeNameIsRequired",
        "Catalog type name is required.");

    public static readonly Error TypeNameTooLong = Error.Problem(
        "CatalogType.TypeNameTooLong",
        "Catalog type name cannot exceed 100 characters.");
}