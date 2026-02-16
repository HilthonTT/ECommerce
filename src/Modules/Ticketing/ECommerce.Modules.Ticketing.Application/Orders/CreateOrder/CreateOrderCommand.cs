using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(
    Guid CustomerId, 
    string City,
    string Street,
    string State,
    string Country,
    string ZipCode,
    string CardNumber,
    string CardHolderName,
    DateTime CardExpiration,
    string CardSecurityNumber,
    List<OrderItemDto> OrderItems) : ICommand;