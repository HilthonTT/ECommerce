using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Catalog.Application.Catalog.CreateItem;

public sealed record CreateItemCommand(
    int CatalogItemId,
    Guid CatalogBrandId,
    Guid CatalogTypeId,
    string Name,
    string? Description,
    string? PictureFileName,
    decimal Price,
    int AvailableStock,
    int RestockThreshold,
    int MaxStockThreshold) : ICommand<CatalogItemResponse>;
