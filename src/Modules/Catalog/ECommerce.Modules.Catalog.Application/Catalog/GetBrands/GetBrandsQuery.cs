using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetBrands;

public sealed record GetBrandsQuery : IQuery<CollectionResponse<CatalogBrandResponse>>;
