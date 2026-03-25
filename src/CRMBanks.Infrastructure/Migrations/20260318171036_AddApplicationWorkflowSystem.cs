using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRMBanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationWorkflowSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "assigned_worker_id",
                table: "loan_applications",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "assigned_worker_id",
                table: "deposit_applications",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "application_internal_notes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    application_id = table.Column<int>(type: "integer", nullable: false),
                    application_type = table.Column<string>(type: "text", nullable: false),
                    worker_id = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false),
                    is_private = table.Column<bool>(type: "boolean", nullable: false),
                    note_type = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    date_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_internal_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_application_internal_notes_users_worker_id",
                        column: x => x.worker_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "verification_checklists",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    application_id = table.Column<int>(type: "integer", nullable: false),
                    application_type = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    is_mandatory = table.Column<bool>(type: "boolean", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_by_worker_id = table.Column<int>(type: "integer", nullable: true),
                    comments = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    date_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verification_checklists", x => x.id);
                    table.ForeignKey(
                        name: "fk_verification_checklists_users_completed_by_worker_id",
                        column: x => x.completed_by_worker_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_loan_applications_assigned_worker_id",
                table: "loan_applications",
                column: "assigned_worker_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_applications_assigned_worker_id",
                table: "deposit_applications",
                column: "assigned_worker_id");

            migrationBuilder.CreateIndex(
                name: "ix_application_internal_notes_worker_id",
                table: "application_internal_notes",
                column: "worker_id");

            migrationBuilder.CreateIndex(
                name: "ix_verification_checklists_completed_by_worker_id",
                table: "verification_checklists",
                column: "completed_by_worker_id");

            migrationBuilder.AddForeignKey(
                name: "fk_deposit_applications_users_assigned_worker_id",
                table: "deposit_applications",
                column: "assigned_worker_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_loan_applications_users_assigned_worker_id",
                table: "loan_applications",
                column: "assigned_worker_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_deposit_applications_users_assigned_worker_id",
                table: "deposit_applications");

            migrationBuilder.DropForeignKey(
                name: "fk_loan_applications_users_assigned_worker_id",
                table: "loan_applications");

            migrationBuilder.DropTable(
                name: "application_internal_notes");

            migrationBuilder.DropTable(
                name: "verification_checklists");

            migrationBuilder.DropIndex(
                name: "ix_loan_applications_assigned_worker_id",
                table: "loan_applications");

            migrationBuilder.DropIndex(
                name: "ix_deposit_applications_assigned_worker_id",
                table: "deposit_applications");

            migrationBuilder.DropColumn(
                name: "assigned_worker_id",
                table: "loan_applications");

            migrationBuilder.DropColumn(
                name: "assigned_worker_id",
                table: "deposit_applications");
        }
    }
}
