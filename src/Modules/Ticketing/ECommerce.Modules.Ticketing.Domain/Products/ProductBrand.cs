using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;

namespace ECommerce.Modules.Ticketing.Domain.Products;

[Auditable]
public sealed class ProductBrand : Entity
{
    public Guid Id { get; private init; }

    public string Brand { get; private set; } = string.Empty;

    private ProductBrand() { }

    public static Result<ProductBrand> Create(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
        {
            return Result.Failure<ProductBrand>(ProductBrandErrors.BrandNameIsRequired);
        }

        if (brand.Length > 100)
        {
            return Result.Failure<ProductBrand>(ProductBrandErrors.BrandNameTooLong);
        }

        var productBrand = new ProductBrand
        {
            Id = Guid.CreateVersion7(),
            Brand = brand.Trim()
        };

        productBrand.RaiseDomainEvent(new ProductBrandCreatedDomainEvent(productBrand.Id, productBrand.Brand));

        return Result.Success(productBrand);
    }

    public Result UpdateBrand(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
        {
            return ProductBrandErrors.BrandNameIsRequired;
        }

        if (brand.Length > 100)
        {
            return ProductBrandErrors.BrandNameTooLong;
        }

        Brand = brand.Trim();

        RaiseDomainEvent(new ProductBrandUpdatedDomainEvent(Id, Brand));

        return Result.Success();
    }
}

[Auditable]
public sealed class CatalogType : Entity
{
    public Guid Id { get; private init; }
    public string Type { get; private set; } = string.Empty;

    private CatalogType() { }

    public static Result<CatalogType> Create(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            return Result.Failure<CatalogType>(CatalogTypeErrors.TypeNameIsRequired);
        }

        if (type.Length > 100)
        {
            return Result.Failure<CatalogType>(CatalogTypeErrors.TypeNameTooLong);
        }

        var catalogType = new CatalogType
        {
            Id = Guid.CreateVersion7(),
            Type = type.Trim()
        };

        catalogType.RaiseDomainEvent(new CatalogTypeCreatedDomainEvent(catalogType.Id, catalogType.Type));

        return Result.Success(catalogType);
    }

    public Result UpdateType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            return CatalogTypeErrors.TypeNameIsRequired;
        }

        if (type.Length > 100)
        {
            return CatalogTypeErrors.TypeNameTooLong;
        }

        Type = type.Trim();

        RaiseDomainEvent(new CatalogTypeUpdatedDomainEvent(Id, Type));

        return Result.Success();
    }
}
