using System.Text.Json.Serialization;

namespace ECommerce.Web.Shared.DTOs.Authentication;

public sealed class AccessTokensDto
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
