using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using Microsoft.Extensions.Logging;

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

        Result<AccessTokensResponse> result = await keyCloakTokenClient.LoginUserAsync(
            loginRepresentation,
            cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Login failed for user {Email}: {Error}",
                login.Email,
                result.Error.Code);
        }

        return result;
    }

    public async Task<Result<string>> RegisterUserAsync(
        UserModel user,
        CancellationToken cancellationToken = default)
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
            string identityId = await keyCloakAdminClient.RegisterUserAsync(
                userRepresentation, cancellationToken);

            return identityId;
        }
        catch (HttpRequestException exception)
            when (exception.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            logger.LogError(exception, "User registration failed");

            return IdentityProviderErrors.EmailIsNotUnique;
        }
    }

    public async Task<Result<AccessTokensResponse>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        Result<AccessTokensResponse> result = await keyCloakTokenClient.RefreshTokenAsync(
            refreshToken,
            cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Token refresh failed: {Error}",
                result.Error.Code);
        }

        return result;
    }
}
