namespace ECommerce.Modules.Catalog.Application.Catalog;

public sealed record CatalogItemResponse
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public decimal Price { get; init; }

    public string? PictureFileName { get; init; }

    public Guid CatalogTypeId { get; init; }

    public CatalogTypeResponse? CatalogType { get; init; }

    public Guid CatalogBrandId { get; init; }

    public CatalogBrandResponse? CatalogBrand { get; init; }

    public int AvailableStock { get; init; }

    public int RestockThreshold { get; init; }

    public int MaxStockThreshold { get; init; }

    public bool OnReorder { get; init; }
}
