using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ECommerce.Modules.Ticketing.Domain.Products;

[Auditable]
public sealed class Product : Entity
{
    public Guid Id { get; private init; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int AvailableStock { get; private set; }
    public int RestockThreshold { get; private set; }
    public int MaxStockThreshold { get; private set; }
    public Guid ProductBrandId { get; private set; }
    public Guid CatalogTypeId { get; private set; }

    public ProductBrand ProductBrand { get; private set; } = default!;
    public ProductType ProductType { get; private set; } = default!;

    [NotMapped, JsonConverter(typeof(EmbeddingJsonConverter))]
    public ReadOnlyMemory<float> NameEmbedding { get; private set; }

    private Product() { }

    public static Result<Product> Create(
        string name,
        decimal price,
        Guid productBrandId,
        Guid catalogTypeId,
        ReadOnlyMemory<float> nameEmbedding,
        string? description = null,
        int availableStock = 0,
        int restockThreshold = 0,
        int maxStockThreshold = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Product>(ProductErrors.NameIsRequired);
        }

        if (name.Length > 200)
        {
            return Result.Failure<Product>(ProductErrors.NameTooLong);
        }

        if (price <= 0)
        {
            return Result.Failure<Product>(ProductErrors.PriceMustBePositive);
        }

        if (availableStock < 0)
        {
            return Result.Failure<Product>(ProductErrors.AvailableStockCannotBeNegative);
        }

        if (restockThreshold < 0)
        {
            return Result.Failure<Product>(ProductErrors.RestockThresholdMustBePositive);
        }

        if (maxStockThreshold < 0)
        {
            return Result.Failure<Product>(ProductErrors.MaxStockThresholdMustBePositive);
        }

        if (maxStockThreshold > 0 && maxStockThreshold <= restockThreshold)
        {
            return Result.Failure<Product>(ProductErrors.MaxStockMustBeGreaterThanRestockThreshold);
        }

        if (productBrandId == Guid.Empty)
        {
            return Result.Failure<Product>(ProductErrors.BrandIdIsRequired);
        }

        if (catalogTypeId == Guid.Empty)
        {
            return Result.Failure<Product>(ProductErrors.CatalogTypeIdIsRequired);
        }

        if (nameEmbedding.IsEmpty)
        {
            return Result.Failure<Product>(ProductErrors.NameEmbeddingIsRequired);
        }

        var product = new Product
        {
            Id = Guid.CreateVersion7(),
            Name = name.Trim(),
            Description = description?.Trim(),
            Price = price,
            AvailableStock = availableStock,
            RestockThreshold = restockThreshold,
            MaxStockThreshold = maxStockThreshold,
            ProductBrandId = productBrandId,
            CatalogTypeId = catalogTypeId,
            NameEmbedding = nameEmbedding
        };

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(
            product.Id,
            product.Name,
            product.Price));

        return Result.Success(product);
    }

    public Result UpdateDetails(string name, string? description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ProductErrors.NameIsRequired;
        }

        if (name.Length > 200)
        {
            return ProductErrors.NameTooLong;
        }

        if (price <= 0)
        {
            return ProductErrors.PriceMustBePositive;
        }

        Name = name.Trim();
        Description = description?.Trim();
        Price = price;

        RaiseDomainEvent(new ProductDetailsUpdatedDomainEvent(Id, Name, Price));

        return Result.Success();
    }

    public Result UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
        {
            return ProductErrors.PriceMustBePositive;
        }

        decimal oldPrice = Price;
        Price = newPrice;

        RaiseDomainEvent(new ProductPriceChangedDomainEvent(Id, oldPrice, newPrice));

        return Result.Success();
    }

    public Result AddStock(int quantity)
    {
        if (quantity <= 0)
        {
            return ProductErrors.AvailableStockCannotBeNegative;
        }

        int previousStock = AvailableStock;
        AvailableStock += quantity;

        RaiseDomainEvent(new ProductStockAddedDomainEvent(Id, quantity, AvailableStock));

        // Check if we're back above restock threshold
        if (previousStock <= RestockThreshold && AvailableStock > RestockThreshold)
        {
            RaiseDomainEvent(new ProductRestockedDomainEvent(Id, AvailableStock));
        }

        return Result.Success();
    }

    public Result<int> RemoveStock(int quantityDesired)
    {
        if (quantityDesired <= 0)
        {
            return Result.Failure<int>(ProductErrors.AvailableStockCannotBeNegative);
        }

        if (AvailableStock == 0)
        {
            return Result.Failure<int>(ProductErrors.InsufficientStock);
        }

        int removedQuantity = Math.Min(quantityDesired, AvailableStock);

        AvailableStock -= removedQuantity;

        RaiseDomainEvent(new ProductStockRemovedDomainEvent(Id, removedQuantity, AvailableStock));

        // Check if we've fallen below restock threshold
        if (AvailableStock <= RestockThreshold)
        {
            RaiseDomainEvent(new ProductRestockThresholdReachedDomainEvent(
                Id,
                AvailableStock,
                RestockThreshold));
        }

        return Result.Success(removedQuantity);
    }

    public Result UpdateStockThresholds(int restockThreshold, int maxStockThreshold)
    {
        if (restockThreshold < 0)
        {
            return ProductErrors.RestockThresholdMustBePositive;
        }

        if (maxStockThreshold < 0)
        {
            return ProductErrors.MaxStockThresholdMustBePositive;
        }

        if (maxStockThreshold > 0 && maxStockThreshold <= restockThreshold)
        {
            return ProductErrors.MaxStockMustBeGreaterThanRestockThreshold;
        }

        RestockThreshold = restockThreshold;
        MaxStockThreshold = maxStockThreshold;

        return Result.Success();
    }

    public bool IsLowStock() => AvailableStock <= RestockThreshold;

    public bool IsOutOfStock() => AvailableStock == 0;
}