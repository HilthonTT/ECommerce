namespace ECommerce.Modules.Ticketing.Application.Abstractions.AI;

public interface IPythonInferenceClient
{
    Task<string?> ClassifyTextAsync(
        string text, 
        IEnumerable<string> candidateLabels, 
        CancellationToken cancellationToken = default);
}
