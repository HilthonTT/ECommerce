using ECommerce.Common.Application.Sorting;
using ECommerce.Modules.Ticketing.Application.Orders.GetOrder;
using ECommerce.Modules.Ticketing.Application.Orders.GetUserOrders;
using ECommerce.Modules.Ticketing.Domain.Orders;
using System.Linq.Expressions;

namespace ECommerce.Modules.Ticketing.Application.Orders;

public static class OrderMappings
{
    public static readonly SortMappingDefinition<OrderSummaryDto, Order> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(OrderSummaryDto.OrderId), nameof(Order.Id)),
            new SortMapping(nameof(OrderSummaryDto.Status), nameof(Order.Status)),
            new SortMapping(nameof(OrderSummaryDto.CreatedAtUtc), nameof(Order.CreatedAtUtc)),
            new SortMapping(nameof(OrderSummaryDto.TotalPrice), nameof(Order.TotalPrice)),
            new SortMapping(nameof(OrderSummaryDto.Currency), nameof(Order.Currency)),
        ]
    };

    internal static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status,
            CreatedAtUtc = order.CreatedAtUtc,
            TotalPrice = order.TotalPrice,
            Description = order.Description,
            Currency = order.Currency,
            Address = new AddressDto
            {
                Street = order.Address.Street,
                City = order.Address.City,
                State = order.Address.State,
                ZipCode = order.Address.ZipCode,
                Country = order.Address.Country,
            },
        };
    }

    internal static OrderSummaryDto ToSummaryDto(this Order order)
    {
        return new OrderSummaryDto
        {
            OrderId = order.Id,
            Status = order.Status,
            CreatedAtUtc = order.CreatedAtUtc,
            TotalPrice = order.TotalPrice,
            Currency = order.Currency,
        };
    }

    internal static Expression<Func<Order, OrderSummaryDto>> ProjectToSummaryDto()
    {
        return order => new OrderSummaryDto
        {
            OrderId = order.Id,
            Status = order.Status,
            CreatedAtUtc = order.CreatedAtUtc,
            TotalPrice = order.TotalPrice,
            Currency = order.Currency,
        };
    }
}
