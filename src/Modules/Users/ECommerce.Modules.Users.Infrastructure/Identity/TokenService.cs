using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Common.Application.Clock;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Modules.Users.Infrastructure.Identity;

internal sealed class TokenService(IOptions<TwoFactorTokenOptions> options, IDateTimeProvider dateTimeProvider) : ITokenService
{
    private const string PurposeClaimType = "purpose";

    public string GenerateLimitedToken(Guid userId, string purpose)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new Claim(PurposeClaimType, purpose)
        };

        var token = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: claims,
            notBefore: dateTimeProvider.UtcNow,
            expires: dateTimeProvider.UtcNow.AddMinutes(options.Value.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public LimitedTokenClaims? ValidateLimitedToken(string token, string expectedPurpose)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SigningKey));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = options.Value.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero // strict expiry for 2FA
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, validationParameters, out _);

            string? userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            string? purpose = principal.FindFirstValue(PurposeClaimType);

            if (string.IsNullOrEmpty(userId) || purpose != expectedPurpose)
            {
                return null;
            }

            return new LimitedTokenClaims(Guid.Parse(userId), purpose);
        }
        catch (SecurityTokenException)
        {
            return null;
        }
    }
}
