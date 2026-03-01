using ECommerce.Common.Application.Sorting;
using ECommerce.Modules.Catalog.Domain.Catalog;
using System.Linq.Expressions;

namespace ECommerce.Modules.Catalog.Application.Catalog;

public static class CatalogItemMappings
{
    public static readonly SortMappingDefinition<CatalogItemResponse, CatalogItem> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(CatalogItemResponse.Id), nameof(CatalogItem.Id)),
            new SortMapping(nameof(CatalogItemResponse.Name), nameof(CatalogItem.Name)),
            new SortMapping(nameof(CatalogItemResponse.Price), nameof(CatalogItem.Price)),
            new SortMapping(nameof(CatalogItemResponse.AvailableStock), nameof(CatalogItem.AvailableStock)),
            new SortMapping(nameof(CatalogItemResponse.CatalogTypeId), nameof(CatalogItem.CatalogTypeId)),
            new SortMapping(nameof(CatalogItemResponse.CatalogBrandId), nameof(CatalogItem.CatalogBrandId)),
        ]
    };

    internal static CatalogItemResponse ToResponse(this CatalogItem item)
    {
        return new CatalogItemResponse
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            PictureFileName = item.PictureFileName,
            CatalogTypeId = item.CatalogTypeId,
            CatalogType = item.CatalogType?.ToResponse(),
            CatalogBrandId = item.CatalogBrandId,
            CatalogBrand = item.CatalogBrand?.ToResponse(),
            AvailableStock = item.AvailableStock,
            RestockThreshold = item.RestockThreshold,
            MaxStockThreshold = item.MaxStockThreshold,
            OnReorder = item.OnReorder,
        };
    }

    internal static Expression<Func<CatalogItem, CatalogItemResponse>> ProjectToResponse()
    {
        return item => new CatalogItemResponse
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            PictureFileName = item.PictureFileName,
            CatalogTypeId = item.CatalogTypeId,
            CatalogType = item.CatalogType == null ? null : new CatalogTypeResponse
            {
                Id = item.CatalogType.Id,
                Type = item.CatalogType.Type,
            },
            CatalogBrandId = item.CatalogBrandId,
            CatalogBrand = item.CatalogBrand == null ? null : new CatalogBrandResponse
            {
                Id = item.CatalogBrand.Id,
                Brand = item.CatalogBrand.Brand,
            },
            AvailableStock = item.AvailableStock,
            RestockThreshold = item.RestockThreshold,
            MaxStockThreshold = item.MaxStockThreshold,
            OnReorder = item.OnReorder,
        };
    }

    private static CatalogTypeResponse ToResponse(this CatalogType type)
    {
        return new CatalogTypeResponse
        {
            Id = type.Id,
            Type = type.Type,
        };
    }

    private static CatalogBrandResponse ToResponse(this CatalogBrand brand)
    {
        return new CatalogBrandResponse
        {
            Id = brand.Id,
            Brand = brand.Brand,
        };
    }
}
