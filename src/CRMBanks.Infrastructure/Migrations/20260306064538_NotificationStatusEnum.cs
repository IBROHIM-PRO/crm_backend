using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMBanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NotificationStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_read",
                table: "notifications");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "notifications");

            migrationBuilder.AddColumn<bool>(
                name: "is_read",
                table: "notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
