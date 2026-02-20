using ECommerce.Modules.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Modules.Users.Infrastructure.Users;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(permission => permission.Code);
        builder.Property(permission => permission.Code).HasMaxLength(100);
        builder.HasData(
            // Users
            Permission.GetUser,
            Permission.ModifyUser,
            // Carts
            Permission.GetCart,
            Permission.ViewCart,
            Permission.AddToCart,
            Permission.RemoveFromCart,
            // Tickets
            Permission.CreateTicket,
            Permission.ViewTickets,
            Permission.UpdateTickets,
            Permission.ChatAssistant,
            // Orders
            Permission.CreateOrder,
            Permission.ViewOrders,
            Permission.ShipOrder,
            Permission.CancelOrder,
            // Webhooks
            Permission.ViewWebhooks,
            Permission.CreateWebhooks,
            Permission.UpdateWebhooks,
            Permission.RemoveWebhooks
        );
        builder.HasMany<Role>()
            .WithMany()
            .UsingEntity(joinBuilder =>
            {
                joinBuilder.ToTable("role_permissions");
                joinBuilder.HasData(
                    // Member permissions
                    CreateRolePermission(Role.Member, Permission.GetUser),
                    CreateRolePermission(Role.Member, Permission.ModifyUser),
                    CreateRolePermission(Role.Member, Permission.GetCart),
                    CreateRolePermission(Role.Member, Permission.ViewCart),
                    CreateRolePermission(Role.Member, Permission.AddToCart),
                    CreateRolePermission(Role.Member, Permission.RemoveFromCart),
                    CreateRolePermission(Role.Member, Permission.CreateTicket),
                    CreateRolePermission(Role.Member, Permission.ViewTickets),
                    CreateRolePermission(Role.Member, Permission.UpdateTickets),
                    CreateRolePermission(Role.Member, Permission.ChatAssistant),
                    CreateRolePermission(Role.Member, Permission.CreateOrder),
                    CreateRolePermission(Role.Member, Permission.ViewOrders),
                    CreateRolePermission(Role.Member, Permission.CancelOrder),
                    // Administrator permissions
                    CreateRolePermission(Role.Administrator, Permission.GetUser),
                    CreateRolePermission(Role.Administrator, Permission.ModifyUser),
                    CreateRolePermission(Role.Administrator, Permission.GetCart),
                    CreateRolePermission(Role.Administrator, Permission.ViewCart),
                    CreateRolePermission(Role.Administrator, Permission.AddToCart),
                    CreateRolePermission(Role.Administrator, Permission.RemoveFromCart),
                    CreateRolePermission(Role.Administrator, Permission.CreateTicket),
                    CreateRolePermission(Role.Administrator, Permission.ViewTickets),
                    CreateRolePermission(Role.Administrator, Permission.UpdateTickets),
                    CreateRolePermission(Role.Administrator, Permission.ChatAssistant),
                    CreateRolePermission(Role.Administrator, Permission.CreateOrder),
                    CreateRolePermission(Role.Administrator, Permission.ViewOrders),
                    CreateRolePermission(Role.Administrator, Permission.ShipOrder),
                    CreateRolePermission(Role.Administrator, Permission.CancelOrder),
                    CreateRolePermission(Role.Administrator, Permission.ViewWebhooks),
                    CreateRolePermission(Role.Administrator, Permission.CreateWebhooks),
                    CreateRolePermission(Role.Administrator, Permission.UpdateWebhooks),
                    CreateRolePermission(Role.Administrator, Permission.RemoveWebhooks)
                );
            });
    }

    private static object CreateRolePermission(Role role, Permission permission)
    {
        return new
        {
            RoleName = role.Name,
            PermissionCode = permission.Code
        };
    }
}
