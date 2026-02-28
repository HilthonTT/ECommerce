using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Users.Application.TwoFactor.Validate;

public sealed record ValidateTwoFactorCommand(Guid UserId, string Code, string LimitedToken) 
    : ICommand<ValidateTwoFactorResponse>;
