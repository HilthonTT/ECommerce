using ECommerce.Common.Application.Encryption;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Abstractions.Data;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using ECommerce.Modules.Users.Application.Abstractions.TwoFactor;
using ECommerce.Modules.Users.Domain.TwoFactor;
using ECommerce.Modules.Users.Domain.Users;

namespace ECommerce.Modules.Users.Application.TwoFactor.Validate;

internal sealed class ValidateTwoFactorCommandHandler(
    IUserRepository userRepository,
    IRecoveryCodeRepository recoveryCodeRepository,
    ITwoFactorService twoFactorService,
    IEncryptionService encryptionService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork)
    : ICommandHandler<ValidateTwoFactorCommand, ValidateTwoFactorResponse>
{
    public async Task<Result<ValidateTwoFactorResponse>> Handle(
        ValidateTwoFactorCommand command,
        CancellationToken cancellationToken)
    {
        LimitedTokenClaims? claims = tokenService.ValidateLimitedToken(
                command.LimitedToken, "2fa");

        if (claims is null)
        {
            return TwoFactorErrors.InvalidLimitedToken;
        }

        User? user = await userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return TwoFactorErrors.UserNotFound;
        }

        if (!user.TwoFactorEnabled || user.TwoFactorSecret is null || user.TwoFactorSecretKey is null)
        {
            return TwoFactorErrors.NotEnabled;
        }

        // Try TOTP first, then fall back to recovery code
        var encryptionResult = new EncryptionResult
        {
            EncryptedData = user.TwoFactorSecret,
            Key = user.TwoFactorSecretKey
        };

        string base32Secret = encryptionService.Decrypt(encryptionResult);

        bool isValid = twoFactorService.VerifyCode(base32Secret, command.Code, out long timeStepMatched);

        if (isValid)
        {
            // Prevent replay attacks
            if (user.LastUsedTimeStep.HasValue && timeStepMatched <= user.LastUsedTimeStep.Value)
            {
                return TwoFactorErrors.CodeAlreadyUsed;
            }

            user.UpdateLastUsedTimeStep(timeStepMatched);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new ValidateTwoFactorResponse(true);
        }

        // Try recovery code
        bool recoveryValid = await recoveryCodeRepository.TryConsumeAsync(
            user.Id, command.Code, cancellationToken);

        if (!recoveryValid)
        {
            return TwoFactorErrors.InvalidCode;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ValidateTwoFactorResponse(true);
    }
}
