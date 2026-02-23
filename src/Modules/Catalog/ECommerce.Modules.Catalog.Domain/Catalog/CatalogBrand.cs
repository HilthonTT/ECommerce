using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;

namespace ECommerce.Modules.Catalog.Domain.Catalog;

[Auditable]
public sealed class CatalogBrand : Entity
{
    public Guid Id { get; private set; }

    public string Brand { get; private set; } = string.Empty;

    public static CatalogBrand Create(string brand)
    {
        return new CatalogBrand { Id = Guid.CreateVersion7(), Brand = brand };
    }
}