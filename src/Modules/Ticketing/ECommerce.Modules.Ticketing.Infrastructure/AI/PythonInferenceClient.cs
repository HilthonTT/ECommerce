using ECommerce.Modules.Ticketing.Application.Abstractions.AI;
using System.Net.Http.Json;

namespace ECommerce.Modules.Ticketing.Infrastructure.AI;

internal sealed class PythonInferenceClient(HttpClient httpClient) : IPythonInferenceClient
{
    public async Task<string?> ClassifyTextAsync(
        string text, 
        IEnumerable<string> candidateLabels,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "/classify",
           new { text, candidate_labels = candidateLabels },
           cancellationToken);

        string? label = await response.Content.ReadFromJsonAsync<string>(cancellationToken);

        return label;
    }
}
