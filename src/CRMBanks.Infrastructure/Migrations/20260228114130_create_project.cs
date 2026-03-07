using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRMBanks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class create_project : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "banks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_banks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cards",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cards", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "regions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "type_credits",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_type_credits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "type_deposits",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_type_deposits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "type_sums",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_type_sums", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    bank_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    az_sum = table.Column<int>(type: "integer", nullable: false),
                    to_sum = table.Column<int>(type: "integer", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    password = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_banks_bank_id",
                        column: x => x.bank_id,
                        principalTable: "banks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "type_products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_credit_id = table.Column<int>(type: "integer", nullable: true),
                    type_deposit_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_type_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_type_products_type_credits_type_credit_id",
                        column: x => x.type_credit_id,
                        principalTable: "type_credits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_type_products_type_deposits_type_deposit_id",
                        column: x => x.type_deposit_id,
                        principalTable: "type_deposits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "credits",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    bank_id = table.Column<int>(type: "integer", nullable: false),
                    type_credit_id = table.Column<int>(type: "integer", nullable: false),
                    foiz = table.Column<double>(type: "double precision", nullable: false),
                    az_sum = table.Column<int>(type: "integer", nullable: false),
                    to_sum = table.Column<int>(type: "integer", nullable: false),
                    document = table.Column<string>(type: "text", nullable: false),
                    type_sum_id = table.Column<string>(type: "text", nullable: false),
                    type_sum_id1 = table.Column<int>(type: "integer", nullable: true),
                    az_sana = table.Column<int>(type: "integer", nullable: false),
                    to_sana = table.Column<int>(type: "integer", nullable: false),
                    infoprot = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_credits", x => x.id);
                    table.ForeignKey(
                        name: "fk_credits_banks_bank_id",
                        column: x => x.bank_id,
                        principalTable: "banks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_credits_type_credits_type_credit_id",
                        column: x => x.type_credit_id,
                        principalTable: "type_credits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_credits_type_sums_type_sum_id1",
                        column: x => x.type_sum_id1,
                        principalTable: "type_sums",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "deposits",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    bank_id = table.Column<int>(type: "integer", nullable: false),
                    type_deposit_id = table.Column<int>(type: "integer", nullable: false),
                    foiz = table.Column<double>(type: "double precision", nullable: false),
                    az_sum = table.Column<int>(type: "integer", nullable: false),
                    to_sum = table.Column<int>(type: "integer", nullable: false),
                    document = table.Column<string>(type: "text", nullable: false),
                    type_sum_id = table.Column<string>(type: "text", nullable: false),
                    type_sum_id1 = table.Column<int>(type: "integer", nullable: true),
                    az_sana = table.Column<int>(type: "integer", nullable: false),
                    to_sana = table.Column<int>(type: "integer", nullable: false),
                    infoprot = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deposits", x => x.id);
                    table.ForeignKey(
                        name: "fk_deposits_banks_bank_id",
                        column: x => x.bank_id,
                        principalTable: "banks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_deposits_type_deposits_type_deposit_id",
                        column: x => x.type_deposit_id,
                        principalTable: "type_deposits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_deposits_type_sums_type_sum_id1",
                        column: x => x.type_sum_id1,
                        principalTable: "type_sums",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "auth2fs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<int>(type: "integer", nullable: false),
                    date_time_send_code = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auth2fs", x => x.id);
                    table.ForeignKey(
                        name: "fk_auth2fs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "region_user",
                columns: table => new
                {
                    regions_id = table.Column<int>(type: "integer", nullable: false),
                    users_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_region_user", x => new { x.regions_id, x.users_id });
                    table.ForeignKey(
                        name: "fk_region_user_regions_regions_id",
                        column: x => x.regions_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_region_user_users_users_id",
                        column: x => x.users_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    credit_id = table.Column<int>(type: "integer", nullable: true),
                    deposit_id = table.Column<int>(type: "integer", nullable: true),
                    card_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_products_cards_card_id",
                        column: x => x.card_id,
                        principalTable: "cards",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_products_credits_credit_id",
                        column: x => x.credit_id,
                        principalTable: "credits",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_products_deposits_deposit_id",
                        column: x => x.deposit_id,
                        principalTable: "deposits",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: true),
                    bank_id = table.Column<int>(type: "integer", nullable: true),
                    sum = table.Column<int>(type: "integer", nullable: false),
                    srok = table.Column<int>(type: "integer", nullable: false),
                    region_id = table.Column<int>(type: "integer", nullable: true),
                    type_product_id = table.Column<int>(type: "integer", nullable: true),
                    state = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_requests_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_requests_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_requests_type_products_type_product_id",
                        column: x => x.type_product_id,
                        principalTable: "type_products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bank_request",
                columns: table => new
                {
                    banks_id = table.Column<int>(type: "integer", nullable: false),
                    requests_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bank_request", x => new { x.banks_id, x.requests_id });
                    table.ForeignKey(
                        name: "fk_bank_request_banks_banks_id",
                        column: x => x.banks_id,
                        principalTable: "banks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bank_request_requests_requests_id",
                        column: x => x.requests_id,
                        principalTable: "requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "request_actions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    request_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    bank_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_request_actions", x => x.id);
                    table.ForeignKey(
                        name: "fk_request_actions_banks_bank_id",
                        column: x => x.bank_id,
                        principalTable: "banks",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_request_actions_requests_request_id",
                        column: x => x.request_id,
                        principalTable: "requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_request_actions_users_user_id",
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
                    { 1, "admin" },
                    { 2, "user" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_auth2fs_user_id",
                table: "auth2fs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_bank_request_requests_id",
                table: "bank_request",
                column: "requests_id");

            migrationBuilder.CreateIndex(
                name: "ix_credits_bank_id",
                table: "credits",
                column: "bank_id");

            migrationBuilder.CreateIndex(
                name: "ix_credits_type_credit_id",
                table: "credits",
                column: "type_credit_id");

            migrationBuilder.CreateIndex(
                name: "ix_credits_type_sum_id1",
                table: "credits",
                column: "type_sum_id1");

            migrationBuilder.CreateIndex(
                name: "ix_deposits_bank_id",
                table: "deposits",
                column: "bank_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposits_type_deposit_id",
                table: "deposits",
                column: "type_deposit_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposits_type_sum_id1",
                table: "deposits",
                column: "type_sum_id1");

            migrationBuilder.CreateIndex(
                name: "ix_products_card_id",
                table: "products",
                column: "card_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_credit_id",
                table: "products",
                column: "credit_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_deposit_id",
                table: "products",
                column: "deposit_id");

            migrationBuilder.CreateIndex(
                name: "ix_region_user_users_id",
                table: "region_user",
                column: "users_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_actions_bank_id",
                table: "request_actions",
                column: "bank_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_actions_request_id",
                table: "request_actions",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_actions_user_id",
                table: "request_actions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_requests_product_id",
                table: "requests",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_requests_region_id",
                table: "requests",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_requests_type_product_id",
                table: "requests",
                column: "type_product_id");

            migrationBuilder.CreateIndex(
                name: "ix_type_products_type_credit_id",
                table: "type_products",
                column: "type_credit_id");

            migrationBuilder.CreateIndex(
                name: "ix_type_products_type_deposit_id",
                table: "type_products",
                column: "type_deposit_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_bank_id",
                table: "users",
                column: "bank_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auth2fs");

            migrationBuilder.DropTable(
                name: "bank_request");

            migrationBuilder.DropTable(
                name: "region_user");

            migrationBuilder.DropTable(
                name: "request_actions");

            migrationBuilder.DropTable(
                name: "requests");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "regions");

            migrationBuilder.DropTable(
                name: "type_products");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "cards");

            migrationBuilder.DropTable(
                name: "credits");

            migrationBuilder.DropTable(
                name: "deposits");

            migrationBuilder.DropTable(
                name: "type_credits");

            migrationBuilder.DropTable(
                name: "banks");

            migrationBuilder.DropTable(
                name: "type_deposits");

            migrationBuilder.DropTable(
                name: "type_sums");
        }
    }
}
