using ECommerce.Modules.Users.Application.Abstractions.Identity;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Security.Authentication;

namespace ECommerce.Modules.Users.Infrastructure.Identity;

internal sealed class KeyCloakTokenClient(
    HttpClient httpClient,
    IOptions<KeyCloakOptions> options)
{
    private readonly KeyCloakOptions _options = options.Value;

    internal Task<AccessTokensResponse> LoginUserAsync(
        LoginRepresentation login,
        CancellationToken cancellationToken = default)
    {
        var authRequestParameters = new KeyValuePair<string, string>[]
        {
            new("client_id", _options.PublicClientId),
            new("scope", "email openid"),
            new("grant_type", "password"),
            new("username", login.Email),
            new("password", login.Password),
        };

        return RequestTokenAsync(authRequestParameters, cancellationToken);
    }

    internal Task<AccessTokensResponse> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var authRequestParameters = new KeyValuePair<string, string>[]
        {
            new("client_id", _options.PublicClientId),
            new("grant_type", "refresh_token"),
            new("refresh_token", refreshToken),
        };

        return RequestTokenAsync(authRequestParameters, cancellationToken);
    }

    private async Task<AccessTokensResponse> RequestTokenAsync(
        KeyValuePair<string, string>[] parameters,
        CancellationToken cancellationToken)
    {
        using var content = new FormUrlEncodedContent(parameters);
        using var request = new HttpRequestMessage(HttpMethod.Post, _options.TokenUrl)
        {
            Content = content
        };

        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => new AuthenticationException("Invalid credentials"),
                HttpStatusCode.BadRequest => new InvalidOperationException($"Invalid request: {errorContent}"),
                _ => new HttpRequestException($"Keycloak request failed: {response.StatusCode}")
            };
        }

        return await response.Content.ReadFromJsonAsync<AccessTokensResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize token response");
    }
}
