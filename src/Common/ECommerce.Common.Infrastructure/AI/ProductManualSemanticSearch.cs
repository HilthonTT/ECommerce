using ECommerce.Common.Application.AI;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Memory;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ECommerce.Common.Infrastructure.AI;

internal sealed class ProductManualSemanticSearch(
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IServiceProvider serviceProvider)
    : IProductManualSemanticSearch
{
    private const string ManualCollectionName = "manuals";
    private const int DefaultResultLimit = 3;

    public async Task<IReadOnlyList<MemoryQueryResult>> SearchAsync(
        Guid? productId,
        string query,
        CancellationToken cancellationToken = default)
    {
        var embeddings = await embeddingGenerator.GenerateAsync([query], cancellationToken: cancellationToken);
        var embedding = embeddings[0].Vector;

        object searchRequest = CreateSearchRequest(embedding, productId);

        using HttpClient httpClient = serviceProvider.GetQdrantHttpClient("vector-db");
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            $"collections/{ManualCollectionName}/points/search",
            searchRequest,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        QdrantResponse? qdrantResult = await response.Content.ReadFromJsonAsync<QdrantResponse>(cancellationToken);

        return MapToMemoryResults(qdrantResult!.Result);
    }

    private static object CreateSearchRequest(ReadOnlyMemory<float> embedding, Guid? productId)
    {
        var request = new
        {
            vector = embedding,
            with_payload = new[] { "id", "text", "external_source_name", "additional_metadata" },
            limit = DefaultResultLimit,
            filter = (object?)null
        };

        if (productId.HasValue)
        {
            return request with
            {
                filter = new
                {
                    must = new[]
                    {
                        new { key = "external_source_name", match = new { value = $"productid:{productId}" } }
                    }
                }
            };
        }

        return request;
    }

    private static List<MemoryQueryResult> MapToMemoryResults(QdrantResultEntry[] entries)
    {
        return entries.Select(entry => new MemoryQueryResult(
            new MemoryRecordMetadata(
                isReference: true,
                id: entry.Payload.Id,
                text: entry.Payload.Text,
                description: string.Empty,
                externalSourceName: entry.Payload.ExternalSourceName,
                additionalMetadata: entry.Payload.AdditionalMetadata),
            relevance: entry.Score,
            embedding: null))
        .ToList();
    }

    private sealed record QdrantResponse
    {
        [JsonPropertyName("result")]
        public required QdrantResultEntry[] Result { get; init; }
    }

    private sealed record QdrantResultEntry
    {
        [JsonPropertyName("score")]
        public required float Score { get; init; }

        [JsonPropertyName("payload")]
        public required QdrantPayload Payload { get; init; }
    }

    private sealed record QdrantPayload
    {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("text")]
        public required string Text { get; init; }

        [JsonPropertyName("external_source_name")]
        public required string ExternalSourceName { get; init; }

        [JsonPropertyName("additional_metadata")]
        public required string AdditionalMetadata { get; init; }
    }
}
