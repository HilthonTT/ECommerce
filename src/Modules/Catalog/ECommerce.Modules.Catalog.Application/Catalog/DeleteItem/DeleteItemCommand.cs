using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Catalog.Application.Catalog.DeleteItem;

public sealed record DeleteItemCommand(int CatalogItemId) : ICommand;
