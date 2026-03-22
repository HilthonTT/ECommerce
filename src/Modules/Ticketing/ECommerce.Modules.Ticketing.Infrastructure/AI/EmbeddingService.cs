using ECommerce.Modules.Ticketing.Application.Abstractions.AI;
using Microsoft.Extensions.AI;

namespace ECommerce.Modules.Ticketing.Infrastructure.AI;

internal sealed class EmbeddingService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    : IEmbeddingService
{
    public async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var embedding = await embeddingGenerator.GenerateAsync(
            text,
            cancellationToken: cancellationToken);

        return embedding.Vector;
    }
}