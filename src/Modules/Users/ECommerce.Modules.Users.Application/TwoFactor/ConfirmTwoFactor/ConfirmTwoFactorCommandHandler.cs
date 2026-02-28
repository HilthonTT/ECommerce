using ECommerce.Common.Application.Encryption;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Abstractions.Data;
using ECommerce.Modules.Users.Application.Abstractions.TwoFactor;
using ECommerce.Modules.Users.Domain.TwoFactor;
using ECommerce.Modules.Users.Domain.Users;

namespace ECommerce.Modules.Users.Application.TwoFactor.ConfirmTwoFactor;

internal sealed class ConfirmTwoFactorCommandHandler(
    IUserRepository userRepository,
    IRecoveryCodeRepository recoveryCodeRepository,
    ITwoFactorService twoFactorService,
    IEncryptionService encryptionService,
    IUnitOfWork unitOfWork)
    : ICommandHandler<ConfirmTwoFactorCommand, ConfirmTwoFactorResponse>
{
    public async Task<Result<ConfirmTwoFactorResponse>> Handle(
        ConfirmTwoFactorCommand command,
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return TwoFactorErrors.UserNotFound;
        }

        if (user.PendingTwoFactorSecret is null || user.PendingTwoFactorSecretKey is null)
        {
            return TwoFactorErrors.NoPendingSetup;
        }

        // Decrypt the pending secret to verify the TOTP code
        var encryptionResult = new EncryptionResult
        {
            EncryptedData = user.PendingTwoFactorSecret,
            Key = user.PendingTwoFactorSecretKey
        };

        string base32Secret = encryptionService.Decrypt(encryptionResult);

        bool isValid = twoFactorService.VerifyCode(base32Secret, command.Code, out _);
        if (!isValid)
        {
            return TwoFactorErrors.InvalidCode;
        }

        // Activate 2FA and generate recovery codes
        user.ActivateTwoFactor();

        List<string> plainCodes = twoFactorService.GenerateRecoveryCodes();
        await recoveryCodeRepository.ReplaceAsync(user.Id, plainCodes, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ConfirmTwoFactorResponse(plainCodes);
    }
}