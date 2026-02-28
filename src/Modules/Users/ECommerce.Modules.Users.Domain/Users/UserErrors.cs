using ECommerce.Common.Domain;

namespace ECommerce.Modules.Users.Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) =>
        Error.NotFound("Users.NotFound", $"The user with the identifier {userId} not found.");

    public static Error NotFound(string identityId) =>
        Error.NotFound("Users.NotFound", $"The user with the IDP identifier {identityId} not found.");

    public static readonly Error TwoFactorNotEnabled = Error.Problem(
        "Users.TwoFactorNotEnabled",
        "2FA is not enabled.");

    public static readonly Error InvalidTwoFactorCode = Error.Problem(
        "Users.InvalidTwoFactorCode",
        "The two factor code was invalid.");
}