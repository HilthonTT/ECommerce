using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Catalog.Application.Catalog.UpdateItem;

public sealed record UpdateItemCommand(int CatalogItemId, string Name, string? Description, decimal Price) : ICommand;
