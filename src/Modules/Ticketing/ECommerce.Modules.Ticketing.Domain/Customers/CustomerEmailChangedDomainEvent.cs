using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Customers;

public sealed class CustomerEmailChangedDomainEvent(
    Guid paymentMethodId,
    string email) : DomainEvent
{
    public Guid PaymentMethodId { get; } = paymentMethodId;

    public string Email { get; } = email;
}
