using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ECommerce.Modules.Users.Infrastructure.Identity;

internal sealed class KeyCloakIdentityProviderService(
    KeyCloakClient keyCloakClient,
    ILogger<KeyCloakIdentityProviderService> logger) : IIdentityProviderService
{
    private const string PasswordCredentialType = "Password";

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
            var identityId = await keyCloakClient.RegisterUserAsync(userRepresentation, cancellationToken);
            return identityId;
        }
        catch (HttpRequestException exception) 
            when (exception.StatusCode == HttpStatusCode.Conflict)
        {
            logger.LogError(exception, "User registration failed");

            return IdentityProviderErrors.EmailIsNotUnique;
        }
    }
}
