using ECommerce.Common.Domain;

namespace ECommerce.Modules.Catalog.Domain.Catalog;

public static class CatalogItemErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "CatalogItem.NotFound",
        $"The catalog item with Id = '{id}' was not found");

    public static readonly Error SoldOut = Error.Problem(
        "CatalogItem.SoldOut",
        "The product is sold out");

    public static readonly Error ItemUnitsGreaterThanZero = Error.Problem(
        "CatalogItem.ItemUnitsGreaterThanZero",
        "Item units must be greater than zero");
}
