using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMBanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_entity_product_for_all_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_deleted_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                table: "requests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_deleted_at",
                table: "requests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated",
                table: "requests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "requests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                table: "request_actions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_deleted_at",
                table: "request_actions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated",
                table: "request_actions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "request_actions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                table: "regions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_deleted_at",
                table: "regions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated",
                table: "regions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "regions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                table: "notifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_deleted_at",
                table: "notifications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated",
                table: "notifications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                table: "deposits",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_deleted_at",
                table: "deposits",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated",
                table: "deposits",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "deposits",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                table: "credits",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_deleted_at",
                table: "credits",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated",
                table: "credits",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "credits",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                table: "cards",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_deleted_at",
                table: "cards",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated",
                table: "cards",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "cards",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                table: "banks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_deleted_at",
                table: "banks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated",
                table: "banks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "banks",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_created",
                table: "users");

            migrationBuilder.DropColumn(
                name: "date_deleted_at",
                table: "users");

            migrationBuilder.DropColumn(
                name: "date_updated",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "users");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "requests");

            migrationBuilder.DropColumn(
                name: "date_deleted_at",
                table: "requests");

            migrationBuilder.DropColumn(
                name: "date_updated",
                table: "requests");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "requests");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "request_actions");

            migrationBuilder.DropColumn(
                name: "date_deleted_at",
                table: "request_actions");

            migrationBuilder.DropColumn(
                name: "date_updated",
                table: "request_actions");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "request_actions");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "date_deleted_at",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "date_updated",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "date_deleted_at",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "date_updated",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "deposits");

            migrationBuilder.DropColumn(
                name: "date_deleted_at",
                table: "deposits");

            migrationBuilder.DropColumn(
                name: "date_updated",
                table: "deposits");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "deposits");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "credits");

            migrationBuilder.DropColumn(
                name: "date_deleted_at",
                table: "credits");

            migrationBuilder.DropColumn(
                name: "date_updated",
                table: "credits");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "credits");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "date_deleted_at",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "date_updated",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "banks");

            migrationBuilder.DropColumn(
                name: "date_deleted_at",
                table: "banks");

            migrationBuilder.DropColumn(
                name: "date_updated",
                table: "banks");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "banks");
        }
    }
}
