using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Products.UpdateProduct;

public sealed record UpdateProductCommand(
    int ProductId,
    string Name,
    decimal Price,
    string? Description) : ICommand;
