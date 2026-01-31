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
            .HasForeignKey(orderItem => orderItem.OrderId);
    }
}
