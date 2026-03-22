using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Products.RemoveStock;

public sealed record RemoveProductStockCommand(
    int ProductId,
    int QuantityRemoved) : ICommand;
