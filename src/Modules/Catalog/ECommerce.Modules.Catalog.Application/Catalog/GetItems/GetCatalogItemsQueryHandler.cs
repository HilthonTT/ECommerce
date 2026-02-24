using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Application.Sorting;
using ECommerce.Common.Domain;
using ECommerce.Modules.Catalog.Application.Abstractions.AI;
using ECommerce.Modules.Catalog.Application.Abstractions.Data;
using ECommerce.Modules.Catalog.Domain.Catalog;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetItems;

internal sealed class GetCatalogItemsQueryHandler(
    IDbContext dbContext,
    ISortMappingProvider sortMappingProvider,
    ICatalogAI catalogAI)
    : IQueryHandler<GetCatalogItemsQuery, PaginationResult<CatalogItemResponse>>
{
    public async Task<Result<PaginationResult<CatalogItemResponse>>> Handle(
        GetCatalogItemsQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<CatalogItem> catalogItemQuery = dbContext.CatalogItems.AsQueryable();

        bool usingVectorSearch = false;

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            if (catalogAI.IsEnabled)
            {
                Vector? vector = await catalogAI.GetEmbeddingAsync(query.SearchTerm, cancellationToken);
                if (vector is not null)
                {
                    catalogItemQuery = catalogItemQuery
                        .Where(c => c.Embedding != null)
                        .OrderBy(c => c.Embedding!.CosineDistance(vector));

                    usingVectorSearch = true;
                }
                else
                {
                    catalogItemQuery = catalogItemQuery.Where(c => c.Name.StartsWith(query.SearchTerm));
                }
            }
            else
            {
                catalogItemQuery = catalogItemQuery.Where(c => c.Name.StartsWith(query.SearchTerm));
            }
        }

        if (query.CatalogBrandId.HasValue)
        {
            catalogItemQuery = catalogItemQuery.Where(c => c.CatalogBrandId == query.CatalogBrandId.Value);
        }

        if (query.CatalogTypeId.HasValue)
        {
            catalogItemQuery = catalogItemQuery.Where(c => c.CatalogTypeId == query.CatalogTypeId.Value);
        }

        var sortMappings = sortMappingProvider.GetMappings<CatalogItemResponse, CatalogItem>();

        var responseQuery = (usingVectorSearch ? catalogItemQuery : catalogItemQuery.ApplySort(query.Sort, sortMappings))
            .Select(CatalogItemMappings.ProjectToResponse());

        var paginationResult = await PaginationResult<CatalogItemResponse>.CreateAsync(
            responseQuery,
            query.Page,
            query.PageSize,
            cancellationToken);

        return paginationResult;
    }
}
