using System.ComponentModel.DataAnnotations;

namespace ECommerce.Modules.Ticketing.Domain.Carts;

public sealed class CartItem : IValidatableObject
{
    public required int ProductId { get; set; }

    public required string ProductName { get; set; } = string.Empty;

    public required decimal UnitPrice { get; set; }

    public required decimal OldUnitPrice { get; set; }

    public required int Quantity { get; set; }

    public required string PictureUrl { get; set; } = string.Empty;

    public required string Currency { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Quantity < 1)
        {
            results.Add(new ValidationResult("Invalid number of units", ["Quantity"]));
        }

        return results;
    }
}