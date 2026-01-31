using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

// Product Events
public sealed class ProductCreatedDomainEvent(Guid productId, string name, decimal price) : DomainEvent
{
    public Guid ProductId { get; } = productId;
    public string Name { get; } = name;
    public decimal Price { get; } = price;
}
