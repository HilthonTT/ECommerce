using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Catalog.Application.Abstractions.Data;
using ECommerce.Modules.Catalog.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetItem;

internal sealed class GetCatalogItemQueryHandler(IDbContext dbContext)
    : IQueryHandler<GetCatalogItemQuery, CatalogItemResponse>
{
    public async Task<Result<CatalogItemResponse>> Handle(
        GetCatalogItemQuery query,
        CancellationToken cancellationToken)
    {
        CatalogItem? catalogItem = await dbContext.CatalogItems
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (catalogItem is null)
        {
            return CatalogItemErrors.NotFound(query.Id);
        }

        return catalogItem.ToResponse();
    }
}
