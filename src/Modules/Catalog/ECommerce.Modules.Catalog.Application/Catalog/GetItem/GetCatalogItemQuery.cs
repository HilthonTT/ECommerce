using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetItem;

public sealed record GetCatalogItemQuery(int Id) : IQuery<CatalogItemResponse>;
