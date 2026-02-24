using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetItems;

public sealed record GetCatalogItemsQuery(
    int Page, 
    int PageSize, 
    string? SearchTerm,
    string? Sort,
    Guid? CatalogTypeId,
    Guid? CatalogBrandId) : IQuery<PaginationResult<CatalogItemResponse>>;
