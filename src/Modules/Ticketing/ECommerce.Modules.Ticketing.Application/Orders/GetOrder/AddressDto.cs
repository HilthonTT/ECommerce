namespace ECommerce.Modules.Ticketing.Application.Orders.GetOrder;

public sealed record AddressDto
{
    public required string Street { get; set; }

    public required string City { get; set; }

    public required string State { get; set; }

    public required string Country { get; set; }

    public required string ZipCode { get; set; }
}