using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;

namespace ECommerce.Modules.Ticketing.Domain.Customers;

[Auditable]
public sealed class PaymentMethod : Entity
{
    public Guid Id { get; private init; }
    public Guid CustomerId { get; private init; }
    public string Alias { get; private set; } = string.Empty;
    public string CardNumber { get; private set; } = string.Empty;
    public string SecurityNumber { get; private set; } = string.Empty;
    public string CardHolderName { get; private set; } = string.Empty;
    public DateTime Expiration { get; private set; }
    public int CardTypeId { get; private set; }
    public CardType CardType { get; private set; } = default!;

    private PaymentMethod() { }

    public static Result<PaymentMethod> Create(
        Guid customerId,
        int cardTypeId,
        string alias,
        string cardNumber,
        string securityNumber,
        string cardHolderName,
        DateTime expiration)
    {
        if (customerId == Guid.Empty)
        {
            return Result.Failure<PaymentMethod>(CustomerErrors.CustomerIdCannotBeEmpty);
        }

        if (string.IsNullOrWhiteSpace(alias))
        {
            return Result.Failure<PaymentMethod>(PaymentMethodErrors.AliasIsRequired);
        }

        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return Result.Failure<PaymentMethod>(PaymentMethodErrors.CardNumberIsRequired);
        }

        if (string.IsNullOrWhiteSpace(securityNumber))
        {
            return Result.Failure<PaymentMethod>(PaymentMethodErrors.SecurityNumberIsRequired);
        }

        if (string.IsNullOrWhiteSpace(cardHolderName))
        {
            return Result.Failure<PaymentMethod>(PaymentMethodErrors.CardHolderNameIsRequired);
        }

        if (cardTypeId <= 0)
        {
            return Result.Failure<PaymentMethod>(PaymentMethodErrors.CardTypeIdIsInvalid);
        }

        if (expiration < DateTime.UtcNow)
        {
            return Result.Failure<PaymentMethod>(PaymentMethodErrors.CardIsExpired);
        }

        var paymentMethod = new PaymentMethod
        {
            Id = Guid.CreateVersion7(),
            CustomerId = customerId,
            Alias = alias,
            CardNumber = cardNumber,
            SecurityNumber = securityNumber,
            CardHolderName = cardHolderName,
            Expiration = expiration,
            CardTypeId = cardTypeId
        };

        paymentMethod.RaiseDomainEvent(new PaymentMethodAddedDomainEvent(
            paymentMethod.Id,
            customerId,
            cardTypeId,
            alias));

        return Result.Success(paymentMethod);
    }

    public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
    {
        return CardTypeId == cardTypeId
            && CardNumber == cardNumber
            && Expiration == expiration;
    }

    public Result Update(string alias, DateTime expiration)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            return PaymentMethodErrors.AliasIsRequired;
        }

        if (expiration < DateTime.UtcNow)
        {
            return PaymentMethodErrors.CardIsExpired;
        }

        Alias = alias;
        Expiration = expiration;

        return Result.Success();
    }
}
