using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRMBanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updat_card_and_create_type_card : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "cards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "cards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "purchase_amount",
                table: "cards",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "shelf_life_months",
                table: "cards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "type_card_id",
                table: "cards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "type_card",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_type_card", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cards_type_card_id",
                table: "cards",
                column: "type_card_id");

            migrationBuilder.AddForeignKey(
                name: "fk_cards_type_card_type_card_id",
                table: "cards",
                column: "type_card_id",
                principalTable: "type_card",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cards_type_card_type_card_id",
                table: "cards");

            migrationBuilder.DropTable(
                name: "type_card");

            migrationBuilder.DropIndex(
                name: "ix_cards_type_card_id",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "description",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "purchase_amount",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "shelf_life_months",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "type_card_id",
                table: "cards");
        }
    }
}
