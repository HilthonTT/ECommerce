using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Customers;

public sealed class PaymentMethodRemovedDomainEvent(
    Guid paymentMethodId,
    Guid customerId) : DomainEvent
{
    public Guid PaymentMethodId { get; } = paymentMethodId;

    public Guid CustomerId { get; } = customerId;
}
