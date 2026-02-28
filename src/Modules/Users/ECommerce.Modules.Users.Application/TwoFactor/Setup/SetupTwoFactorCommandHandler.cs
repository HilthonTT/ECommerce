using ECommerce.Common.Application.Encryption;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Abstractions.Data;
using ECommerce.Modules.Users.Application.Abstractions.TwoFactor;
using ECommerce.Modules.Users.Domain.TwoFactor;
using ECommerce.Modules.Users.Domain.Users;

namespace ECommerce.Modules.Users.Application.TwoFactor.Setup;

internal sealed class SetupTwoFactorCommandHandler(
    IUserRepository userRepository,
    ITwoFactorService twoFactorService,
    IEncryptionService encryptionService,
    IUnitOfWork unitOfWork) 
    : ICommandHandler<SetupTwoFactorCommand, SetupTwoFactorResponse>
{
    // TODO: Add actual issuer
    private const string Issuer = "MyApp";

    public async Task<Result<SetupTwoFactorResponse>> Handle(
        SetupTwoFactorCommand command, 
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return TwoFactorErrors.UserNotFound;
        }

        if (user.TwoFactorEnabled)
        {
            return TwoFactorErrors.AlreadyEnabled;
        }

        var (base32Secret, qrCodeImage) = twoFactorService.GenerateSetup(Issuer, command.UserEmail);

        EncryptionResult encryptionResult = encryptionService.Encrypt(base32Secret);
        user.SetPendingTwoFactorSecret(encryptionResult.EncryptedData, encryptionResult.Key);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SetupTwoFactorResponse(qrCodeImage, base32Secret);
    }
}
