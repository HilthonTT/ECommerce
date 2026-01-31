using ECommerce.Modules.Ticketing.Domain.Orders;
using ECommerce.Modules.Ticketing.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Modules.Ticketing.Infrastructure.Orders;

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedNever();

        builder
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(oi => oi.ProductId);
    }
}
