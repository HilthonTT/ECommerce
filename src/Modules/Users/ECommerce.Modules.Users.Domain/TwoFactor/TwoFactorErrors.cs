using ECommerce.Common.Domain;

namespace ECommerce.Modules.Users.Domain.TwoFactor;

public static class TwoFactorErrors
{
    public static readonly Error NoPendingSetup = Error.Problem("TwoFactor.NoPendingSetup", "No pending 2FA setup found.");
    public static readonly Error InvalidCode = Error.Problem("TwoFactor.InvalidCode", "Invalid or expired code.");
    public static readonly Error CodeAlreadyUsed = Error.Conflict("TwoFactor.CodeAlreadyUsed", "This code has already been used.");
    public static readonly Error NotEnabled = Error.Problem("TwoFactor.NotEnabled", "2FA is not enabled for this user.");
    public static readonly Error AlreadyEnabled = Error.Conflict("TwoFactor.AlreadyEnabled", "2FA is already enabled.");
    public static readonly Error UserNotFound = Error.NotFound("TwoFactor.UserNotFound", "User not found.");
    public static readonly Error InvalidRecoveryCode = Error.Problem("TwoFactor.InvalidRecoveryCode", "Invalid recovery code.");

    public static readonly Error InvalidLimitedToken = Error.Problem("TwoFactor.InvalidLimitedToken", "Invalid limited token.");
}