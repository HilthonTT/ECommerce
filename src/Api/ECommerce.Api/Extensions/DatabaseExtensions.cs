//using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Infrastructure.Database;
using ECommerce.Modules.Catalog.Infrastructure.Database;
//using ECommerce.Modules.Ticketing.Application.Products.CreateProduct;
//using ECommerce.Modules.Ticketing.Application.Products.CreateProductBrand;
//using ECommerce.Modules.Ticketing.Application.Products.CreateProductType;
using ECommerce.Modules.Ticketing.Infrastructure.Database;
using ECommerce.Modules.Users.Infrastructure.Database;
using ECommerce.Webhooks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Extensions;

internal static class DatabaseExtensions
{
    internal static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        await ApplyMigrationsAsync<CatalogDbContext>(scope);
        await ApplyMigrationsAsync<WebhooksDbContext>(scope);
        await ApplyMigrationsAsync<UsersDbContext>(scope);
        await ApplyMigrationsAsync<TicketingDbContext>(scope);

        //using var seedScope = app.ApplicationServices.CreateScope();

        //await TriggerProductEvents(seedScope);
    }

    internal static async Task InvokeSeedAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        await InvokeSeedAsync<CatalogDbContext>(scope);
    }

    private static async Task ApplyMigrationsAsync<TDbContext>(IServiceScope scope)
        where TDbContext : DbContext
    {
        using var context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        await context.Database.MigrateAsync();
    }

    private static async Task InvokeSeedAsync<TDbContext>(IServiceScope scope)
         where TDbContext : DbContext
    {
        using var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<IDbSeeder<TDbContext>>();

        await seeder.SeedAsync(context);
    }

    //private static async Task TriggerProductEvents(IServiceScope scope)
    //{
    //    var ticketingContext = scope.ServiceProvider.GetRequiredService<TicketingDbContext>();

    //    bool hasProducts = await ticketingContext.Products.AnyAsync();

    //    if (hasProducts)
    //    {
    //        return;
    //    }

    //    var catalogContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

    //    var allBrands = await catalogContext.CatalogBrands.ToListAsync();
    //    var allTypes = await catalogContext.CatalogTypes.ToListAsync();
    //    var allProducts = await catalogContext.CatalogItems.ToListAsync();

    //    var createProductBrandHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateProductBrandCommand>>();
    //    var createProductTypeHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateProductTypeCommand>>();
    //    var createProductHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateProductCommand>>();

    //    if (!await ticketingContext.ProductBrands.AnyAsync())
    //    {
    //        foreach (var brand in allBrands)
    //        {
    //            await createProductBrandHandler.Handle(
    //                new CreateProductBrandCommand(brand.Id, brand.Brand),
    //                CancellationToken.None);
    //        }
    //    }

    //    if (!await ticketingContext.ProductTypes.AnyAsync())
    //    {
    //        foreach (var type in allTypes)
    //        {
    //            await createProductTypeHandler.Handle(
    //                new CreateProductTypeCommand(type.Id, type.Type),
    //                CancellationToken.None);
    //        }
    //    }

    //    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseExtensions");

    //    foreach (var product in allProducts)
    //    {
    //        var result = await createProductHandler.Handle(
    //            new CreateProductCommand(
    //                product.Id,
    //                product.Name,
    //                product.Price,
    //                product.CatalogBrandId,
    //                product.CatalogTypeId,
    //                product.Description,
    //                product.AvailableStock,
    //                product.RestockThreshold,
    //                product.MaxStockThreshold),
    //            CancellationToken.None);

    //        if (result.IsFailure)
    //        {
    //            logger.LogWarning(
    //                "Skipping product {ProductId} ({Name}): {Error}",
    //                product.Id, product.Name, result.Error);
    //        }
    //    }
    //}
}
