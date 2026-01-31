using ECommerce.Modules.Ticketing.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Modules.Ticketing.Infrastructure.Products;

internal sealed class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
{
    public void Configure(EntityTypeBuilder<ProductType> builder)
    {
        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.Type)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(pt => pt.Type)
            .IsUnique();
    }
}