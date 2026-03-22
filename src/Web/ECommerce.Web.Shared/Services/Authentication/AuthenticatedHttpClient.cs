using ECommerce.Web.Shared.Services.Common;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ECommerce.Web.Shared.Services.Authentication;

public sealed class AuthenticatedHttpClient(
    IHttpClientFactory httpClientFactory,
    TokenStore tokenStore)
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(HttpClientFactoryNames.Default);
    private readonly TokenStore _tokenStore = tokenStore;

    public async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        AttachToken(request);
        return await _client.SendAsync(request, cancellationToken);
    }

    public async Task<T?> GetFromJsonAsync<T>(
        string url,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        AttachToken(request);
        var response = await _client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync<T>(
        string url,
        T value,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(value)
        };
        AttachToken(request);
        return await _client.SendAsync(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> PutAsJsonAsync<T>(
        string url,
        T value,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = JsonContent.Create(value)
        };
        AttachToken(request);
        return await _client.SendAsync(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> DeleteAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        AttachToken(request);
        return await _client.SendAsync(request, cancellationToken);
    }

    private void AttachToken(HttpRequestMessage request)
    {
        if (_tokenStore.Tokens?.AccessToken is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStore.Tokens.AccessToken);
        }
    }
}
