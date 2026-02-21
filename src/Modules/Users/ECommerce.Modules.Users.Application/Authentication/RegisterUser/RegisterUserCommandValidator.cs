using FluentValidation;

namespace ECommerce.Modules.Users.Application.Authentication.RegisterUser;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(command => command.Email).NotEmpty().EmailAddress();
        RuleFor(command => command.Password).NotEmpty().MinimumLength(6);
        RuleFor(command => command.FirstName).NotEmpty();
        RuleFor(command => command.LastName).NotEmpty();

        RuleFor(x => x.ConfirmPassword).Matches(x => x.Password).WithMessage("The passwords do not match");
    }
}
