using FluentValidation;

namespace ECommerce.Modules.Users.Application.TwoFactor.Validate;

internal sealed class ValidateTwoCommandValidator : AbstractValidator<ValidateTwoFactorCommand>
{
    public ValidateTwoCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
    }
}
