namespace ECommerce.Modules.Ticketing.Application.Abstractions.AI;

public interface IEmbeddingService
{
    Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default);
}