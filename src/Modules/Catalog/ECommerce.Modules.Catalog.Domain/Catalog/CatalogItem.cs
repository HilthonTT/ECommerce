using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;
using NpgsqlTypes;

namespace ECommerce.Modules.Catalog.Domain.Catalog;

[Auditable]
public sealed class CatalogItem : Entity
{
    public int Id { get; private init; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public string? PictureFileName { get; private set; }

    public Guid CatalogTypeId { get; private set; }

    public CatalogType? CatalogType { get; private set; }

    public Guid CatalogBrandId { get; private set; }

    public CatalogBrand? CatalogBrand { get; private set; }

    public int AvailableStock { get; private set; }

    public int RestockThreshold { get; private set; }

    public int MaxStockThreshold { get; private set; }

    public bool OnReorder { get; private set; }

    public NpgsqlTsVector SearchVector { get; set; }

    private CatalogItem() { }

    public static CatalogItem Create(
        int id,
        string name,
        string? description,
        decimal price,
        Guid catalogTypeId,
        Guid catalogBrandId,
        int availableStock,
        int restockThreshold,
        int maxStockThreshold,
        string? pictureFileName)
    {
        var item = new CatalogItem
        {
            Id = id,
            Name = name,
            Description = description,
            Price = price,
            CatalogTypeId = catalogTypeId,
            CatalogBrandId = catalogBrandId,
            AvailableStock = availableStock,
            RestockThreshold = restockThreshold,
            MaxStockThreshold = maxStockThreshold,
            OnReorder = false,
            PictureFileName = pictureFileName,
        };

        item.RaiseDomainEvent(new CatalogItemCreatedDomainEvent(
            item.Id,
            item.Name,
            item.Price,
            item.CatalogTypeId,
            item.CatalogBrandId));

        return item;
    }

    public Result UpdateDetails(string name, string? description, decimal price)
    {
        var previousPrice = Price;

        Name = name;
        Description = description;
        Price = price;

        if (previousPrice != price)
        {
            RaiseDomainEvent(new CatalogItemPriceChangedDomainEvent(Id, previousPrice, price));
        }

        RaiseDomainEvent(new CatalogItemUpdatedDomainEvent(Id, name, description, price));

        return Result.Success();
    }

    /// <summary>
    /// Decrements the quantity of a particular item in inventory and ensures the restockThreshold hasn't
    /// been breached. If so, a RestockRequest is generated in CheckThreshold. 
    /// 
    /// If there is sufficient stock of an item, then the integer returned at the end of this call should be the same as quantityDesired. 
    /// In the event that there is not sufficient stock available, the method will remove whatever stock is available and return that quantity to the client.
    /// In this case, it is the responsibility of the client to determine if the amount that is returned is the same as quantityDesired.
    /// It is invalid to pass in a negative number. 
    /// </summary>
    /// <param name="quantityDesired"></param>
    /// <returns>int: Returns the number actually removed from stock. </returns>
    /// 
    public Result<int> RemoveStock(int quantityDesired)
    {
        if (AvailableStock == 0)
        {
            return CatalogItemErrors.SoldOut;
        }

        if (quantityDesired <= 0)
        {
            return CatalogItemErrors.ItemUnitsGreaterThanZero;
        }

        int removed = Math.Min(quantityDesired, AvailableStock);
        AvailableStock -= removed;

        RaiseDomainEvent(new CatalogItemStockRemovedDomainEvent(Id, removed, AvailableStock));

        if (AvailableStock <= RestockThreshold && !OnReorder)
        {
            OnReorder = true;
            RaiseDomainEvent(new CatalogItemRestockThresholdReachedDomainEvent(Id, AvailableStock, RestockThreshold));
        }

        if (AvailableStock == 0)
        {
            RaiseDomainEvent(new CatalogItemSoldOutDomainEvent(Id));
        }

        return removed;
    }

    /// <summary>
    /// Increments the quantity of a particular item in inventory.
    /// <param name="quantity"></param>
    /// <returns>int: Returns the quantity that has been added to stock</returns>
    /// </summary>
    public Result<int> AddStock(int quantity)
    {
        if (quantity <= 0)
        {
            return CatalogItemErrors.ItemUnitsGreaterThanZero;
        }

        int original = AvailableStock;

        AvailableStock = (AvailableStock + quantity) > MaxStockThreshold
            ? MaxStockThreshold
            : AvailableStock + quantity;

        int added = AvailableStock - original;
        OnReorder = false;

        RaiseDomainEvent(new CatalogItemStockAddedDomainEvent(Id, added, AvailableStock));

        return added;
    }
}
