using ECommerce.Modules.Catalog.Domain.Catalog;
using Pgvector;

namespace ECommerce.Modules.Catalog.Application.Abstractions.AI;

public interface ICatalogAI
{
    bool IsEnabled { get; }

    Task<Vector?> GetEmbeddingAsync(string text, CancellationToken cancellationToken = default);

    Task<Vector?> GetEmbeddingAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Vector>> GetEmbeddingsAsync(
        IEnumerable<CatalogItem> catalogItems, 
        CancellationToken cancellationToken = default);
}
