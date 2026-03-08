using Dapper;
using ECommerce.Common.Application.Data;
using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using System.Data.Common;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetTypes;

internal sealed class GetCatalogTypesQueryHandler(IDbConnectionFactory dbConnectionFactory) 
    : IQueryHandler<GetCatalogTypesQuery, CollectionResponse<CatalogTypeResponse>>
{
    public async Task<Result<CollectionResponse<CatalogTypeResponse>>> Handle(
        GetCatalogTypesQuery query, 
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            $"""
            SELECT id AS {nameof(CatalogTypeResponse.Id)}, 
            type AS {nameof(CatalogTypeResponse.Type)}
            FROM catalog.catalog_types
            ORDER BY type
            """;

        var brands = await connection.QueryAsync<CatalogTypeResponse>(sql);

        return new CollectionResponse<CatalogTypeResponse>
        {
            Items = brands.ToList(),
        };
    }
}
