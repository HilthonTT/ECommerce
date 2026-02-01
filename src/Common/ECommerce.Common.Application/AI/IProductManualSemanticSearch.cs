using Microsoft.SemanticKernel.Memory;

namespace ECommerce.Common.Application.AI;

public interface IProductManualSemanticSearch
{
    Task<IReadOnlyList<MemoryQueryResult>> SearchAsync(Guid? productId, string query, CancellationToken cancellationToken = default);
}
