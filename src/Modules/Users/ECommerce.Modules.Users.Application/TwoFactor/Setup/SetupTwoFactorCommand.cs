using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Users.Application.TwoFactor.Setup;

public sealed record SetupTwoFactorCommand(Guid UserId, string UserEmail)
    : ICommand<SetupTwoFactorResponse>;
