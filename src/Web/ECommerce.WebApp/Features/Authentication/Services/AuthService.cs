using ECommerce.WebApp.Features.Authentication.Models;

namespace ECommerce.WebApp.Features.Authentication.Services;

internal sealed class AuthService(IHttpClientFactory httpClientFactory) : IAuthService
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("ECommerceApi");

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var response = await _client.PostAsJsonAsync("api/v1/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public async Task<User?> RegisterAsync(
        string email, 
        string password, 
        string confirmPassword,
        string firstName,
        string lastName)
    {
        var response = await _client.PostAsJsonAsync("api/v1/auth/register", new
        {
            email,
            password,
            confirmPassword,
            firstName,
            lastName
        });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<User>();
    }

    public async Task<AccessTokensResponse?> RefreshAsync(string refreshToken)
    {
        var response = await _client.PostAsJsonAsync("api/v1/auth/refresh", new { refreshToken });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AccessTokensResponse>();
    }
}
