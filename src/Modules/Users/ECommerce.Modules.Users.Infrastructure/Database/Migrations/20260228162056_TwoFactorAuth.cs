using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Modules.Users.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class TwoFactorAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "last_used_time_step",
                schema: "users",
                table: "users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pending_two_factor_secret",
                schema: "users",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pending_two_factor_secret_key",
                schema: "users",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "two_factor_enabled",
                schema: "users",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "two_factor_secret",
                schema: "users",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "two_factor_secret_key",
                schema: "users",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "recovery_codes",
                schema: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code_hash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recovery_codes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_recovery_codes_user_id",
                schema: "users",
                table: "recovery_codes",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recovery_codes",
                schema: "users");

            migrationBuilder.DropColumn(
                name: "last_used_time_step",
                schema: "users",
                table: "users");

            migrationBuilder.DropColumn(
                name: "pending_two_factor_secret",
                schema: "users",
                table: "users");

            migrationBuilder.DropColumn(
                name: "pending_two_factor_secret_key",
                schema: "users",
                table: "users");

            migrationBuilder.DropColumn(
                name: "two_factor_enabled",
                schema: "users",
                table: "users");

            migrationBuilder.DropColumn(
                name: "two_factor_secret",
                schema: "users",
                table: "users");

            migrationBuilder.DropColumn(
                name: "two_factor_secret_key",
                schema: "users",
                table: "users");
        }
    }
}
