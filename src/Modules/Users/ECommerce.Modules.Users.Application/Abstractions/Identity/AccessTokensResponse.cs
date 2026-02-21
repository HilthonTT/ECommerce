using System.Text.Json.Serialization;

namespace ECommerce.Modules.Users.Application.Abstractions.Identity;

public sealed record AccessTokensResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int AccesssTokenExpiresIn { get; init; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; } = string.Empty;

    [JsonPropertyName("refresh_expires_in")]
    public int RefreshTokenExpiresIn { get; init; }
}