using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Customers;

public sealed class CustomerCreatedDomainEvent(Guid customerId) : DomainEvent
{
    public Guid CustomerId { get; } = customerId;
}