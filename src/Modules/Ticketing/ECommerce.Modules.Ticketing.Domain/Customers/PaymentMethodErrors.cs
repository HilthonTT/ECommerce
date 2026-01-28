using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Customers;

public static class PaymentMethodErrors
{
    public static readonly Error CardNumberIsRequired = Error.Problem(
        "PaymentMethod.CardNumberIsRequired",
        "Card number is required.");

    public static readonly Error SecurityNumberIsRequired = Error.Problem(
        "PaymentMethod.SecurityNumberIsRequired",
        "Security number is required.");

    public static readonly Error CardHolderNameIsRequired = Error.Problem(
        "PaymentMethod.CardHolderNameIsRequired",
        "Card holder name is required.");

    public static readonly Error CardTypeIdIsInvalid = Error.Problem(
        "PaymentMethod.CardTypeIdIsInvalid",
        "Card type ID must be greater than zero.");

    public static readonly Error CardIsExpired = Error.Problem(
        "PaymentMethod.CardIsExpired",
        "The payment card has expired.");

    public static readonly Error AliasIsRequired = Error.Problem(
        "PaymentMethod.AliasIsRequired",
        "Payment method alias is required.");
}