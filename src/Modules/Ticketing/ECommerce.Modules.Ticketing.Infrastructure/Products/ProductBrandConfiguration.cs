using ECommerce.Modules.Ticketing.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Modules.Ticketing.Infrastructure.Products;

internal sealed class ProductBrandConfiguration : IEntityTypeConfiguration<ProductBrand>
{
    public void Configure(EntityTypeBuilder<ProductBrand> builder)
    {
        builder.HasKey(pb => pb.Id);

        builder.Property(pb => pb.Brand)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(pb => pb.Brand)
            .IsUnique();
    }
}