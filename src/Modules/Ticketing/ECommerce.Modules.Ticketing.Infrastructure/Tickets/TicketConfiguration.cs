using ECommerce.Modules.Ticketing.Domain.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Modules.Ticketing.Infrastructure.Tickets;

internal sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(t => t.Code)
            .IsUnique();

        builder.Property(t => t.CustomerId)
            .IsRequired();

        builder.Property(t => t.ProductId)
            .IsRequired(false);

        builder.HasOne(t => t.Product)
            .WithMany()
            .HasForeignKey(t => t.ProductId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.Property(t => t.CreatedAtUtc)
            .IsRequired();

        builder.Property(t => t.ShortSummary)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(t => t.LongSummary)
            .HasMaxLength(5000)
            .IsRequired(false);

        builder.Property(t => t.CustomerSatisfaction)
            .IsRequired(false);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Archived)
            .IsRequired();

        builder.HasOne(t => t.Customer)
            .WithMany()
            .HasForeignKey(t => t.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Product)
            .WithMany()
            .HasForeignKey(t => t.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasMany(t => t.Messages)
            .WithOne()
            .HasForeignKey(m => m.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
