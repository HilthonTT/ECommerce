using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Customers;

public sealed class PaymentMethodAddedToCustomerDomainEvent(
    Guid customerId,
    Guid paymentMethodId,
    Guid orderId) : DomainEvent
{
    public Guid PaymentMethodId { get; } = paymentMethodId;

    public Guid CustomerId { get; } = customerId;

    public Guid OrderId { get; } = orderId;
}

