using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRMBanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepositApplicationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_banks_loan_applications_loan_application_id",
                table: "banks");

            migrationBuilder.DropIndex(
                name: "ix_banks_loan_application_id",
                table: "banks");

            migrationBuilder.DropColumn(
                name: "loan_application_id",
                table: "banks");

            migrationBuilder.CreateTable(
                name: "deposit_applications",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    applicant_name = table.Column<string>(type: "text", nullable: false),
                    applicant_phone = table.Column<string>(type: "text", nullable: false),
                    applicant_email = table.Column<string>(type: "text", nullable: false),
                    deposit_id = table.Column<int>(type: "integer", nullable: false),
                    deposit_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    deposit_term_months = table.Column<int>(type: "integer", nullable: false),
                    region_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    rejection_reason = table.Column<string>(type: "text", nullable: true),
                    application_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    send_to_all_banks = table.Column<bool>(type: "boolean", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    date_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deposit_applications", x => x.id);
                    table.ForeignKey(
                        name: "fk_deposit_applications_deposits_deposit_id",
                        column: x => x.deposit_id,
                        principalTable: "deposits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_deposit_applications_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanApplicationBanks",
                columns: table => new
                {
                    loan_application_id = table.Column<int>(type: "integer", nullable: false),
                    selected_banks_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_loan_application_banks", x => new { x.loan_application_id, x.selected_banks_id });
                    table.ForeignKey(
                        name: "fk_loan_application_banks_banks_selected_banks_id",
                        column: x => x.selected_banks_id,
                        principalTable: "banks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_loan_application_banks_loan_applications_loan_application_id",
                        column: x => x.loan_application_id,
                        principalTable: "loan_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deposit_application_actions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    deposit_application_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    action = table.Column<string>(type: "text", nullable: false),
                    comments = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deposit_application_actions", x => x.id);
                    table.ForeignKey(
                        name: "fk_deposit_application_actions_deposit_applications_deposit_ap",
                        column: x => x.deposit_application_id,
                        principalTable: "deposit_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_deposit_application_actions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepositApplicationBanks",
                columns: table => new
                {
                    deposit_application_id = table.Column<int>(type: "integer", nullable: false),
                    selected_banks_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deposit_application_banks", x => new { x.deposit_application_id, x.selected_banks_id });
                    table.ForeignKey(
                        name: "fk_deposit_application_banks_banks_selected_banks_id",
                        column: x => x.selected_banks_id,
                        principalTable: "banks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_deposit_application_banks_deposit_applications_deposit_applic",
                        column: x => x.deposit_application_id,
                        principalTable: "deposit_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_deposit_application_actions_deposit_application_id",
                table: "deposit_application_actions",
                column: "deposit_application_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_application_actions_user_id",
                table: "deposit_application_actions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_applications_deposit_id",
                table: "deposit_applications",
                column: "deposit_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_applications_region_id",
                table: "deposit_applications",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_application_banks_selected_banks_id",
                table: "DepositApplicationBanks",
                column: "selected_banks_id");

            migrationBuilder.CreateIndex(
                name: "ix_loan_application_banks_selected_banks_id",
                table: "LoanApplicationBanks",
                column: "selected_banks_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deposit_application_actions");

            migrationBuilder.DropTable(
                name: "DepositApplicationBanks");

            migrationBuilder.DropTable(
                name: "LoanApplicationBanks");

            migrationBuilder.DropTable(
                name: "deposit_applications");

            migrationBuilder.AddColumn<int>(
                name: "loan_application_id",
                table: "banks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_banks_loan_application_id",
                table: "banks",
                column: "loan_application_id");

            migrationBuilder.AddForeignKey(
                name: "fk_banks_loan_applications_loan_application_id",
                table: "banks",
                column: "loan_application_id",
                principalTable: "loan_applications",
                principalColumn: "id");
        }
    }
}
