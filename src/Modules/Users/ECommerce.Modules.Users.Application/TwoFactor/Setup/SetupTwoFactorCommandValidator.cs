using FluentValidation;

namespace ECommerce.Modules.Users.Application.TwoFactor.Setup;

internal sealed class SetupTwoFactorCommandValidator : AbstractValidator<SetupTwoFactorCommand>
{
    public SetupTwoFactorCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.UserEmail).NotEmpty().EmailAddress();
    }
}
