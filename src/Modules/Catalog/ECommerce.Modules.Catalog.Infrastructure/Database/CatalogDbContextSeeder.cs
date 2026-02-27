using ECommerce.Common.Infrastructure.Database;
using ECommerce.Modules.Catalog.Domain.Catalog;
using ECommerce.Modules.Catalog.Infrastructure.Catalog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Text.Json;

namespace ECommerce.Modules.Catalog.Infrastructure.Database;

internal sealed class CatalogDbContextSeeder(
    IWebHostEnvironment webHostEnvironment,
    IOptions<CatalogOptions> options,
    ILogger<CatalogDbContextSeeder> logger) : IDbSeeder<CatalogDbContext>
{
    public async Task SeedAsync(CatalogDbContext context)
    {
        bool useCustomizationData = options.Value.UseCustomizationData;
        string contentRootPath = webHostEnvironment.ContentRootPath;
        string picturePath = webHostEnvironment.WebRootPath;

        // Workaround from https://github.com/npgsql/efcore.pg/issues/292#issuecomment-388608426
        await context.Database.OpenConnectionAsync();
        ((NpgsqlConnection)context.Database.GetDbConnection()).ReloadTypes();

        if (!await context.CatalogItems.AnyAsync())
        {
            string sourcePath = Path.Combine(contentRootPath, "Setup", "catalog.json");
            string sourceJson = File.ReadAllText(sourcePath);
            var sourceItems = JsonSerializer.Deserialize<CatalogSourceEntry[]>(sourceJson) ?? [];

            context.CatalogBrands.RemoveRange(context.CatalogBrands);
            await context.CatalogBrands.AddRangeAsync(sourceItems.Select(x => x.Brand).Distinct()
              .Where(brandName => brandName != null)
              .Select(brandName => CatalogBrand.Create(brandName!)));

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Seeded catalog with {NumBrands} brands", context.CatalogBrands.Count());
            }

            context.CatalogTypes.RemoveRange(context.CatalogTypes);
            await context.CatalogTypes.AddRangeAsync(sourceItems.Select(x => x.Type).Distinct()
                .Where(typeName => typeName != null)
                .Select(typeName => CatalogType.Create(typeName!)));

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Seeded catalog with {NumTypes} types", context.CatalogTypes.Count());
            }

            await context.SaveChangesAsync();

            var brandIdsByName = await context.CatalogBrands.ToDictionaryAsync(x => x.Brand, x => x.Id);
            var typeIdsByName = await context.CatalogTypes.ToDictionaryAsync(x => x.Type, x => x.Id);

            CatalogItem[] catalogItems = sourceItems
               .Where(source => source.Name != null && source.Brand != null && source.Type != null)
               .Select(source => CatalogItem.Create(
                   source.Id,
                   source.Name!,
                   source.Description,
                   source.Price,
                   brandIdsByName[source.Brand!],
                   typeIdsByName[source.Type!],
                   100,
                   200,
                   10,
                   $"{source.Id}.webp"))
               .ToArray();

            await context.CatalogItems.AddRangeAsync(catalogItems);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Seeded catalog with {NumItems} items", context.CatalogItems.Count());
            }

            await context.SaveChangesAsync();
        }
    }

    private sealed class CatalogSourceEntry
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? Brand { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
