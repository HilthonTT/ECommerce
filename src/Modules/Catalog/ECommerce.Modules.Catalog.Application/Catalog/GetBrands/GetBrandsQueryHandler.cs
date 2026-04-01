using Dapper;
using ECommerce.Common.Application.Data;
using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Links;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using Microsoft.AspNetCore.Http;
using System.Data.Common;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetBrands;

internal sealed class GetBrandsQueryHandler(IDbConnectionFactory dbConnectionFactory, ILinkService linkService) 
    : IQueryHandler<GetBrandsQuery, CollectionResponse<CatalogBrandResponse>>
{
    public async Task<Result<CollectionResponse<CatalogBrandResponse>>> Handle(
        GetBrandsQuery query, 
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            $"""
            SELECT id AS {nameof(CatalogBrandResponse.Id)}, 
            brand AS {nameof(CatalogBrandResponse.Brand)}
            FROM catalog.catalog_brands
            ORDER BY brand
            """;

        var brands = await connection.QueryAsync<CatalogBrandResponse>(sql);

        return new CollectionResponse<CatalogBrandResponse>
        {
            Items = brands.ToList(),
            Links =
            [
                linkService.CreateForEndpoint(CatalogEndpointNames.GetBrands, "self", HttpMethods.Get)
            ]
        };
    }
}
