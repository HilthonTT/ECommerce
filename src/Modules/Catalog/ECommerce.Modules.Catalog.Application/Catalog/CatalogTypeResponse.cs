namespace ECommerce.Modules.Catalog.Application.Catalog;

public sealed record CatalogTypeResponse
{
    public Guid Id { get; init; }

    public string Type { get; init; } = string.Empty;
}