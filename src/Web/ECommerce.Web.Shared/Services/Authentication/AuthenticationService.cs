using ECommerce.Web.Shared.DTOs.Authentication;
using ECommerce.Web.Shared.Services.Authentication.Interfaces;
using ECommerce.Web.Shared.Services.Common;
using System.Net.Http.Json;

namespace ECommerce.Web.Shared.Services.Authentication;

internal sealed class AuthenticationService(IHttpClientFactory httpClientFactory) : IAuthenticationService
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(HttpClientFactoryNames.Default);

    public async Task<LoginDto?> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("api/v1/auth/login", new { email, password }, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LoginDto>(cancellationToken);
    }

    public async Task<AccessTokensDto?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("api/v1/auth/refresh", new { refreshToken }, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AccessTokensDto>(cancellationToken);
    }

    public async Task<UserDto?> RegisterAsync(
        string email, 
        string password, 
        string confirmPassword, 
        string firstName, 
        string lastName, 
        CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("api/v1/auth/register", new
        {
            email,
            password,
            confirmPassword,
            firstName,
            lastName
        },
        cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken);
    }
}
