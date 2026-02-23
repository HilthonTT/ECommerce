using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public sealed class ProductDetailsUpdatedDomainEvent(int productId, string name, decimal price) : DomainEvent
{
    public int ProductId { get; } = productId;
    public string Name { get; } = name;
    public decimal Price { get; } = price;
}
