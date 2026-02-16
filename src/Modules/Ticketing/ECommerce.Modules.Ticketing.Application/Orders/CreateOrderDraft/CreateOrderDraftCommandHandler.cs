using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Carts.GetCart;
using ECommerce.Modules.Ticketing.Domain.Orders;

namespace ECommerce.Modules.Ticketing.Application.Orders.CreateOrderDraft;

internal sealed class CreateOrderDraftCommandHandler : ICommandHandler<CreateOrderDraftCommand, OrderDraftDto>
{
    public async Task<Result<OrderDraftDto>> Handle(CreateOrderDraftCommand command, CancellationToken cancellationToken)
    {
        var order = Order.NewDraft();
       
        foreach (CartItemDto item in command.Items)
        {
            order.AddItem(
                item.Currency, 
                item.ProductId, 
                item.ProductName,
                item.UnitPrice, 
                item.OldUnitPrice - item.UnitPrice, 
                item.PictureUrl, 
                item.Quantity);
        }

        var dto = new OrderDraftDto
        {
            OrderItems = order.OrderItems.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId,
                ProductName = oi.ProductName,
                UnitPrice = oi.UnitPrice,
                PictureUrl = oi.PictureUrl,
                Units = oi.Units,
                Discount = oi.Discount,
            }).ToList(),
            Total = order.TotalPrice
        };

        return dto;
    }
}
