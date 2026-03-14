namespace ECommerce.Web.Shared.DTOs.Catalog;

public sealed class CatalogItemTypeDto
{
    public required Guid Id { get; init; }

    public required string Type { get; init; }
}