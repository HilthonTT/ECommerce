using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Users.Application.TwoFactor.ConfirmTwoFactor;

public sealed record ConfirmTwoFactorCommand(Guid UserId, string Code)
    : ICommand<ConfirmTwoFactorResponse>;
