namespace ECommerce.Web.Shared.DTOs.Catalog;

public sealed class CatalogItemDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public required decimal Price { get; set; }

    public string? PictureFileName { get; set; }

    public required Guid CatalogBrandId { get; set; }

    public required CatalogBrandDto CatalogBrand { get; set; }

    public required CatalogItemTypeDto CatalogType { get; set; }
}
