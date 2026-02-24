namespace ECommerce.Modules.Catalog.Application.Catalog;

public sealed record CatalogBrandResponse
{
    public Guid Id { get; init; }

    public string Brand { get; init; } = string.Empty;
}
