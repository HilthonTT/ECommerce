using ECommerce.Modules.Catalog.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Modules.Catalog.Infrastructure.Catalog;

internal sealed class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.Property(ci => ci.Name)
           .HasMaxLength(50);

        builder.HasOne(ci => ci.CatalogBrand)
            .WithMany();

        builder.HasOne(ci => ci.CatalogType)
            .WithMany();

        builder.HasGeneratedTsVectorColumn(
            ci => ci.SearchVector,
            "english",
            ci => new { ci.Name, ci.Description, ci.Id })
        .HasIndex(b => b.SearchVector)
        .HasMethod("GIN");
    }
}
