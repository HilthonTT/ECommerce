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
        CatalogItemResponse? response = await dbContext.CatalogItems
            .AsNoTracking()
            .Where(c => c.Id == query.Id)
            .Select(CatalogItemMappings.ProjectToResponse())
            .FirstOrDefaultAsync(cancellationToken);

        return response is null
            ? CatalogItemErrors.NotFound(query.Id)
            : response;
    }
}
