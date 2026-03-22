using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Products.CreateProduct;

public sealed record CreateProductCommand(
    int ProductId,
    string Name,
    decimal Price,
    Guid ProductBrandId,
    Guid ProductTypeId,
    string? Description,
    int AvailableStock,
    int RestockThreshold,
    int MaxStockThreshold) : ICommand;