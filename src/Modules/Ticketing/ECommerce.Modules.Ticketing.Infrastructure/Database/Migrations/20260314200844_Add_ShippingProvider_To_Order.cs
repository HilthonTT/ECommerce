using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Modules.Ticketing.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_ShippingProvider_To_Order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "shipping_cost",
                schema: "ticketing",
                table: "orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "shipping_provider",
                schema: "ticketing",
                table: "orders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shipping_cost",
                schema: "ticketing",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "shipping_provider",
                schema: "ticketing",
                table: "orders");
        }
    }
}
