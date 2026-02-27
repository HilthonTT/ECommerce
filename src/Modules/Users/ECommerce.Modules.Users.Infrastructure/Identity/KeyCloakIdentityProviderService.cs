using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ECommerce.Modules.Users.Infrastructure.Identity;

internal sealed class KeyCloakIdentityProviderService(
    KeyCloakAdminClient keyCloakAdminClient,
    KeyCloakTokenClient keyCloakTokenClient,
    ILogger<KeyCloakIdentityProviderService> logger) : IIdentityProviderService
{
    private const string PasswordCredentialType = "Password";

    public async Task<Result<AccessTokensResponse>> LoginUserAsync(
        LoginModel login,
        CancellationToken cancellationToken = default)
    {
        var loginRepresentation = new LoginRepresentation(
            login.Email,
            login.Password);

        try
        {
            AccessTokensResponse tokens = await keyCloakTokenClient.LoginUserAsync(
                loginRepresentation,
                cancellationToken);

            return tokens;
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.Unauthorized)
        {
            logger.LogWarning(
                exception,
                "Login failed for user {Email}",
                login.Email);

            return Result.Failure<AccessTokensResponse>(IdentityProviderErrors.InvalidCredentials);
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(
                exception,
                "Login request failed for user {Email}",
                login.Email);

            return Result.Failure<AccessTokensResponse>(IdentityProviderErrors.AuthenticationFailed);
        }
    }

    public async Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var userRepresentation = new UserRepresentation(
            user.Email,
            user.Email,
            user.FirstName,
            user.LastName,
            true,
            true,
            [
                new CredentialRepresentation(PasswordCredentialType, user.Password, false),
            ]);

        try
        {
            var identityId = await keyCloakAdminClient.RegisterUserAsync(userRepresentation, cancellationToken);
            return identityId;
        }
        catch (HttpRequestException exception) 
            when (exception.StatusCode == HttpStatusCode.Conflict)
        {
            logger.LogError(exception, "User registration failed");

            return IdentityProviderErrors.EmailIsNotUnique;
        }
    }

    public async Task<Result<AccessTokensResponse>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            AccessTokensResponse tokens = await keyCloakTokenClient.RefreshTokenAsync(
                refreshToken,
                cancellationToken);

            return Result.Success(tokens);
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.BadRequest)
        {
            logger.LogWarning(
                exception,
                "Token refresh failed - invalid or expired refresh token");

            return Result.Failure<AccessTokensResponse>(IdentityProviderErrors.InvalidRefreshToken);
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(
                exception,
                "Token refresh request failed");

            return Result.Failure<AccessTokensResponse>(IdentityProviderErrors.TokenRefreshFailed);
        }
    }
}
