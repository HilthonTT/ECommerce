using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;

namespace ECommerce.Modules.Ticketing.Domain.Orders;

[Auditable]
public sealed class OrderItem : Entity
{
    public string ProductName { get; private set; }

    public string PictureUrl { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Discount { get; private set; }

    public int Units { get; private set; }

    public Guid ProductId { get; private set; }

    private OrderItem() { }

    internal static Result<OrderItem> Create(
        Guid productId, 
        string productName, 
        decimal unitPrice, 
        decimal discount, 
        string pictureUrl, 
        int units = 1)
    {
        if (units <= 0)
        {
            return OrderErrors.InvalidDiscount;
        }

        if ((unitPrice * units) < discount)
        {
            return OrderErrors.TotalOrderItemLowerThanAppliedDiscount;
        }

        return new OrderItem
        {
            ProductId = productId,
            ProductName = productName,
            UnitPrice = unitPrice,
            Discount = discount,
            Units = units,
            PictureUrl = pictureUrl,
        };
    }

    public Result SetNewDiscount(decimal discount)
    {
        if (discount < 0)
        {
            return OrderErrors.InvalidDiscount;
        }

        Discount = discount;

        return Result.Success();
    }

    public Result AddUnits(int units)
    {
        if (units < 0)
        {
            return OrderErrors.InvalidUnits;
        }

        Units += units;

        return Result.Success();
    }
}
