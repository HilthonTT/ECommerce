using ECommerce.Common.Infrastructure.Database;
using ECommerce.Modules.Users.Application.Abstractions.Data;
using ECommerce.Modules.Users.Domain.TwoFactor;
using ECommerce.Modules.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Users.Infrastructure.Database;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) 
    : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users { get; set; }

    public DbSet<RecoveryCode> RecoveryCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Users);

        modelBuilder.ApplyConfigurationsFromAssembly(Common.Infrastructure.AssemblyReference.Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}
