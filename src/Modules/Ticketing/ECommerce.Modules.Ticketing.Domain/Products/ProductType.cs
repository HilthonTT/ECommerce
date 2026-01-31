using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;

namespace ECommerce.Modules.Ticketing.Domain.Products;

[Auditable]
public sealed class ProductType : Entity
{
    public Guid Id { get; private init; }
    public string Type { get; private set; } = string.Empty;

    private ProductType() { }

    public static Result<ProductType> Create(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            return Result.Failure<ProductType>(ProductTypeErrors.TypeNameIsRequired);
        }

        if (type.Length > 100)
        {
            return Result.Failure<ProductType>(ProductTypeErrors.TypeNameTooLong);
        }

        var catalogType = new ProductType
        {
            Id = Guid.CreateVersion7(),
            Type = type.Trim()
        };

        catalogType.RaiseDomainEvent(new ProductTypeCreatedDomainEvent(catalogType.Id, catalogType.Type));

        return Result.Success(catalogType);
    }

    public Result UpdateType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            return ProductTypeErrors.TypeNameIsRequired;
        }

        if (type.Length > 100)
        {
            return ProductTypeErrors.TypeNameTooLong;
        }

        Type = type.Trim();

        RaiseDomainEvent(new ProductTypeUpdatedDomainEvent(Id, Type));

        return Result.Success();
    }
}
