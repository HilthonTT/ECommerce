using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;

namespace ECommerce.Modules.Catalog.Domain.Catalog;

[Auditable]
public sealed class CatalogType : Entity
{
    public Guid Id { get; private set; }

    public string Type { get; private set; } = string.Empty;

    public static CatalogType Create(string type)
    {
        return new CatalogType { Id = Guid.CreateVersion7(), Type = type };
    }
}
