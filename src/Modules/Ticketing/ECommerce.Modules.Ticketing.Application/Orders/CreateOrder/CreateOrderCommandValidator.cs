using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Orders.CreateOrder;

internal sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        // Address Information
        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street address is required.")
            .MaximumLength(200)
            .WithMessage("Street address must not exceed 200 characters.");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required.")
            .MaximumLength(100)
            .WithMessage("City must not exceed 100 characters.")
            .Matches("^[a-zA-Z\\s'-]+$")
            .WithMessage("City can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State/Province is required.")
            .MaximumLength(100)
            .WithMessage("State/Province must not exceed 100 characters.");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required.")
            .Length(2)
            .WithMessage("Country must be a 2-letter ISO code (e.g., US, GB, DE).")
            .Matches("^[A-Z]{2}$")
            .WithMessage("Country must be a valid 2-letter uppercase ISO code.");

        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage("Zip/Postal code is required.")
            .MaximumLength(20)
            .WithMessage("Zip/Postal code must not exceed 20 characters.")
            .Matches("^[a-zA-Z0-9\\s-]+$")
            .WithMessage("Zip/Postal code contains invalid characters.");

        // Payment Card Information
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .WithMessage("Card number is required.")
            .CreditCard()
            .WithMessage("Card number is not valid.")
            .Must(BeValidLuhn)
            .WithMessage("Card number failed validation check.");

        RuleFor(x => x.CardHolderName)
            .NotEmpty()
            .WithMessage("Card holder name is required.")
            .MaximumLength(100)
            .WithMessage("Card holder name must not exceed 100 characters.")
            .Matches("^[a-zA-Z\\s'-]+$")
            .WithMessage("Card holder name can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.CardExpiration)
            .NotEmpty()
            .WithMessage("Card expiration date is required.")
            .Must(BeInTheFuture)
            .WithMessage("Card has expired.");

        RuleFor(x => x.CardSecurityNumber)
            .NotEmpty()
            .WithMessage("Card security code (CVV/CVC) is required.")
            .Matches("^[0-9]{3,4}$")
            .WithMessage("Card security code must be 3 or 4 digits.");
    }

    private static bool BeValidLuhn(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return false;
        }

        // Remove spaces and dashes
        cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

        if (!cardNumber.All(char.IsDigit))
        {
            return false;
        }

        // Luhn Algorithm
        int sum = 0;
        bool alternate = false;

        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = cardNumber[i] - '0';

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

    private static bool BeInTheFuture(DateTime expirationDate)
    {
        // Card is valid if it expires at the end of the expiration month
        var endOfExpirationMonth = new DateTime(
            expirationDate.Year,
            expirationDate.Month,
            DateTime.DaysInMonth(expirationDate.Year, expirationDate.Month),
            23, 59, 59);

        return endOfExpirationMonth >= DateTime.UtcNow;
    }
}
