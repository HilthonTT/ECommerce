using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetTypes;

public sealed record GetCatalogTypesQuery : IQuery<CollectionResponse<CatalogTypeResponse>>;
