using ECommerce.Modules.Catalog.Application.Abstractions.AI;
using ECommerce.Modules.Catalog.Domain.Catalog;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Pgvector;
using System.Diagnostics;

namespace ECommerce.Modules.Catalog.Infrastructure.AI;

internal sealed class CatalogAI(
    IEmbeddingGenerator<string, Embedding<float>>? embeddingGenerator,
    ILogger<CatalogAI> logger) : ICatalogAI
{
    private const int EmbeddingDimensions = 384;
    private static readonly ActivitySource ActivitySource = new("ECommerce.Catalog.AI");

    public bool IsEnabled => embeddingGenerator is not null;

    public async Task<Vector?> GetEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        if (!IsEnabled || embeddingGenerator is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            logger.LogWarning("Attempted to generate embedding for empty or whitespace text");
            return null;
        }

        using Activity? activity = ActivitySource.StartActivity("GenerateEmbedding");
        activity?.SetTag("text.length", text.Length);

        long timestamp = Stopwatch.GetTimestamp();

        try
        {
            ReadOnlyMemory<float> embedding = await embeddingGenerator.GenerateVectorAsync(
                text,
                cancellationToken: cancellationToken);

            if (embedding.Length < EmbeddingDimensions)
            {
                logger.LogWarning(
                    "Generated embedding has fewer dimensions than expected. Expected: {Expected}, Got: {Got}",
                    EmbeddingDimensions,
                    embedding.Length);

                return null;
            }

            ReadOnlyMemory<float> truncated = embedding[..EmbeddingDimensions];

            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace(
                    "Generated embedding in {Elapsed:F3}s for text of length {Length}",
                    Stopwatch.GetElapsedTime(timestamp).TotalSeconds,
                    text.Length);
            }

            return new Vector(truncated);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to generate embedding for text of length {Length}", text.Length);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            return null;
        }
    }

    public async Task<Vector?> GetEmbeddingAsync(
        CatalogItem catalogItem,
        CancellationToken cancellationToken = default)
    {
        if (!IsEnabled)
        {
            return null;
        }

        return await GetEmbeddingAsync(CatalogItemToEmbeddingText(catalogItem), cancellationToken);
    }

    public async Task<IReadOnlyList<Vector>> GetEmbeddingsAsync(
        IEnumerable<CatalogItem> catalogItems,
        CancellationToken cancellationToken = default)
    {
        if (!IsEnabled || embeddingGenerator is null)
        {
            return [];
        }

        List<CatalogItem> itemList = catalogItems.ToList();

        if (itemList.Count == 0)
        {
            return [];
        }

        using Activity? activity = ActivitySource.StartActivity("GenerateEmbeddings");
        activity?.SetTag("items.count", itemList.Count);

        long timestamp = Stopwatch.GetTimestamp();

        try
        {
            GeneratedEmbeddings<Embedding<float>> embeddings = await embeddingGenerator.GenerateAsync(
                itemList.Select(CatalogItemToEmbeddingText),
                cancellationToken: cancellationToken);

            List<Vector> results = embeddings
                .Where(e => e.Vector.Length >= EmbeddingDimensions)
                .Select(e => new Vector(e.Vector[..EmbeddingDimensions]))
                .ToList();

            if (results.Count != itemList.Count)
            {
                logger.LogWarning(
                    "Generated {ResultCount} embeddings but expected {ExpectedCount}. Some items may have been skipped",
                    results.Count,
                    itemList.Count);
            }

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(
                    "Generated {Count} embeddings in {Elapsed:F3}s",
                    results.Count,
                    Stopwatch.GetElapsedTime(timestamp).TotalSeconds);
            }

            return results;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to generate embeddings for {Count} catalog items", itemList.Count);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            return [];
        }
    }

    private static string CatalogItemToEmbeddingText(CatalogItem item) =>
        string.IsNullOrWhiteSpace(item.Description)
            ? item.Name
            : $"{item.Name} {item.Description}";
}
