using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Products.CreateProductType;

public sealed record CreateProductTypeCommand(Guid ProductTypeId, string Type) : ICommand;
