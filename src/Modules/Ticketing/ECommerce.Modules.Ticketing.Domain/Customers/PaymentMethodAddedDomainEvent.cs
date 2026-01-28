using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Customers;

public sealed class PaymentMethodAddedDomainEvent(
    Guid paymentMethodId,
    Guid customerId,
    int cardTypeId,
    string alias) : DomainEvent
{
    public Guid PaymentMethodId { get; } = paymentMethodId;

    public Guid CustomerId { get; } = customerId;

    public int CartTypeId { get; } = cardTypeId;

    public string Alias { get; } = alias;
}
