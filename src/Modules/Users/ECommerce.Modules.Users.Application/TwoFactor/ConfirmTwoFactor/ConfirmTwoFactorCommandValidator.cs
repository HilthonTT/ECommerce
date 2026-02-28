using FluentValidation;

namespace ECommerce.Modules.Users.Application.TwoFactor.ConfirmTwoFactor;

internal sealed class ConfirmTwoFactorCommandValidator : AbstractValidator<ConfirmTwoFactorCommand>
{
    public ConfirmTwoFactorCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Code).NotEmpty();
    }
}