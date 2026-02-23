namespace ECommerce.Modules.Catalog.Infrastructure.Catalog;

internal sealed class CatalogOptions
{
    public string? PicBaseUrl { get; init; }

    public required bool UseCustomizationData { get; set; }
}
