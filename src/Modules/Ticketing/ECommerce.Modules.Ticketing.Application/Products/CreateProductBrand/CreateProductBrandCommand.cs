using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Products.CreateProductBrand;

public sealed record CreateProductBrandCommand(Guid ProductBrandId, string Brand) : ICommand;
