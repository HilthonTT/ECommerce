using ECommerce.Modules.Users.Domain.TwoFactor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Modules.Users.Infrastructure.TwoFactor;

internal sealed class RecoveryCodeConfiguration : IEntityTypeConfiguration<RecoveryCode>
{
    public void Configure(EntityTypeBuilder<RecoveryCode> builder)
    {
        builder.HasKey(rc => rc.Id);
        builder.Property(rc => rc.CodeHash).HasMaxLength(100).IsRequired();
        builder.HasIndex(rc => rc.UserId);
    }
}
