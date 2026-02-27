using ECommerce.Modules.Ticketing.Domain.Customers;
using ECommerce.Modules.Ticketing.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Modules.Ticketing.Infrastructure.Orders;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(order => order.Id);

        builder.HasOne<Customer>().WithMany().HasForeignKey(order => order.CustomerId);

        builder.HasMany(order => order.OrderItems)
            .WithOne()
            .HasForeignKey(orderItem => orderItem.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(o => o.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street).HasMaxLength(200).IsRequired();
            addressBuilder.Property(a => a.City).HasMaxLength(100).IsRequired();
            addressBuilder.Property(a => a.State).HasMaxLength(100).IsRequired();
            addressBuilder.Property(a => a.Country).HasMaxLength(100).IsRequired();
            addressBuilder.Property(a => a.ZipCode).HasMaxLength(20).IsRequired();
        });

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.Description)
            .HasMaxLength(500);

        builder.Property(o => o.TotalPrice)
            .HasPrecision(18, 2);

        builder.Property(o => o.Currency)
            .HasMaxLength(10);

        builder.Property(o => o.CreatedAtUtc)
            .IsRequired();
    }
}
