using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public static class ProductErrors
{
    public static readonly Error NameIsRequired = Error.Problem(
        "Product.NameIsRequired",
        "Product name is required.");

    public static readonly Error NameTooLong = Error.Problem(
        "Product.NameTooLong",
        "Product name cannot exceed 200 characters.");

    public static readonly Error PriceMustBePositive = Error.Problem(
        "Product.PriceMustBePositive",
        "Product price must be greater than zero.");

    public static readonly Error AvailableStockCannotBeNegative = Error.Problem(
        "Product.AvailableStockCannotBeNegative",
        "Available stock cannot be negative.");

    public static readonly Error RestockThresholdMustBePositive = Error.Problem(
        "Product.RestockThresholdMustBePositive",
        "Restock threshold must be greater than zero.");

    public static readonly Error MaxStockThresholdMustBePositive = Error.Problem(
        "Product.MaxStockThresholdMustBePositive",
        "Max stock threshold must be greater than zero.");

    public static readonly Error MaxStockMustBeGreaterThanRestockThreshold = Error.Problem(
        "Product.MaxStockMustBeGreaterThanRestockThreshold",
        "Max stock threshold must be greater than restock threshold.");

    public static readonly Error InsufficientStock = Error.Problem(
        "Product.InsufficientStock",
        "Insufficient stock available.");

    public static Error CannotRemoveStock(int available, int requested) => Error.Problem(
        "Product.CannotRemoveStock",
        $"Cannot remove {requested} units. Only {available} units available.");

    public static readonly Error NameEmbeddingIsRequired = Error.Problem(
        "Product.NameEmbeddingIsRequired",
        "Name embedding is required.");

    public static readonly Error BrandIdIsRequired = Error.Problem(
        "Product.BrandIdIsRequired",
        "Product brand ID is required.");

    public static readonly Error CatalogTypeIdIsRequired = Error.Problem(
        "Product.CatalogTypeIdIsRequired",
        "Catalog type ID is required.");
}