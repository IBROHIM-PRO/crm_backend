using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRMBanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanApplicationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "max_loan_amount",
                table: "users",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "loan_application_id",
                table: "banks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "loan_applications",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    applicant_name = table.Column<string>(type: "text", nullable: false),
                    applicant_phone = table.Column<string>(type: "text", nullable: false),
                    applicant_email = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    credit_id = table.Column<int>(type: "integer", nullable: false),
                    requested_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    requested_term_months = table.Column<int>(type: "integer", nullable: false),
                    region_id = table.Column<int>(type: "integer", nullable: false),
                    application_purpose = table.Column<string>(type: "text", nullable: false),
                    monthly_income = table.Column<string>(type: "text", nullable: false),
                    employment_status = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("pk_loan_applications", x => x.id);
                    table.ForeignKey(
                        name: "fk_loan_applications_credits_credit_id",
                        column: x => x.credit_id,
                        principalTable: "credits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_loan_applications_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_loan_applications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "loan_application_actions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    loan_application_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    action = table.Column<string>(type: "text", nullable: false),
                    comments = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_loan_application_actions", x => x.id);
                    table.ForeignKey(
                        name: "fk_loan_application_actions_loan_applications_loan_application",
                        column: x => x.loan_application_id,
                        principalTable: "loan_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_loan_application_actions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 3, "boss" },
                    { 4, "worker" },
                    { 5, "client" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_banks_loan_application_id",
                table: "banks",
                column: "loan_application_id");

            migrationBuilder.CreateIndex(
                name: "ix_loan_application_actions_loan_application_id",
                table: "loan_application_actions",
                column: "loan_application_id");

            migrationBuilder.CreateIndex(
                name: "ix_loan_application_actions_user_id",
                table: "loan_application_actions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_loan_applications_credit_id",
                table: "loan_applications",
                column: "credit_id");

            migrationBuilder.CreateIndex(
                name: "ix_loan_applications_region_id",
                table: "loan_applications",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_loan_applications_user_id",
                table: "loan_applications",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_banks_loan_applications_loan_application_id",
                table: "banks",
                column: "loan_application_id",
                principalTable: "loan_applications",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_banks_loan_applications_loan_application_id",
                table: "banks");

            migrationBuilder.DropTable(
                name: "loan_application_actions");

            migrationBuilder.DropTable(
                name: "loan_applications");

            migrationBuilder.DropIndex(
                name: "ix_banks_loan_application_id",
                table: "banks");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "max_loan_amount",
                table: "users");

            migrationBuilder.DropColumn(
                name: "loan_application_id",
                table: "banks");
        }
    }
}
