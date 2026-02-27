using ECommerce.Modules.Ticketing.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Modules.Ticketing.Infrastructure.Products;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.AvailableStock)
            .IsRequired();

        builder.Property(p => p.RestockThreshold)
            .IsRequired();

        builder.Property(p => p.MaxStockThreshold)
            .IsRequired();

        builder.Property(p => p.ProductBrandId)
            .IsRequired();

        builder.Property(p => p.CatalogTypeId)
            .IsRequired();

        builder.Property(p => p.NameEmbedding)
            .HasColumnType("vector(1536)")
            .IsRequired();

        builder.HasOne(p => p.ProductBrand)
            .WithMany()
            .HasForeignKey(p => p.ProductBrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ProductType)
            .WithMany()
            .HasForeignKey(p => p.CatalogTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.Name);

        builder.Ignore(p => p.NameEmbedding);
    }
}
