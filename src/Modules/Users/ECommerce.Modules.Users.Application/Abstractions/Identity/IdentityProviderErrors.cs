using ECommerce.Common.Domain;

namespace ECommerce.Modules.Users.Application.Abstractions.Identity;

public static class IdentityProviderErrors
{
    public static readonly Error EmailIsNotUnique =
        Error.Conflict(
            "Identity.EmailIsNotUnique",
            "The specified email is not unique");

    public static readonly Error InvalidCredentials =
     Error.Problem(
         "Identity.InvalidCredentials",
         "The provided credentials are invalid");

    public static readonly Error AuthenticationFailed =
        Error.Problem(
            "Identity.AuthenticationFailed",
            "Authentication request failed");

    public static readonly Error InvalidRefreshToken =
        Error.Problem(
            "Identity.InvalidRefreshToken",
            "The refresh token is invalid or expired");

    public static readonly Error TokenRefreshFailed =
        Error.Problem(
            "Identity.TokenRefreshFailed",
            "Token refresh request failed");
}
