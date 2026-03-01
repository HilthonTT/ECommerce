namespace ECommerce.WebApp.Features.Catalog.Models;

public sealed record CatalogItem
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public decimal Price { get; init; }

    public string PictureUrl { get; init; } = string.Empty;

    public Guid CatalogBrandId { get; init; }

    public CatalogBrand CatalogBrand { get; init; } = default!;

    public CatalogItemType CatalogType { get; init; } = default!;
}
