using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Mandarin.Database.Migrations
{
    /// <summary>
    /// The initial migration of the existing database schema into Entity Framework.
    /// </summary>
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inventory");

            migrationBuilder.EnsureSchema(
                name: "billing");

            migrationBuilder.EnsureSchema(
                name: "static");

            migrationBuilder.CreateTable(
                name: "commission_rate_group",
                schema: "billing",
                columns: table => new
                {
                    group_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rate = table.Column<int>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("commission_rate_group_pkey", x => x.group_id);
                });

            migrationBuilder.CreateTable(
                name: "status",
                schema: "static",
                columns: table => new
                {
                    status_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status_code = table.Column<string>(maxLength: 25, nullable: false),
                    description = table.Column<string>(maxLength: 100, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status", x => x.status_id);
                    table.UniqueConstraint("AK_status_status_code", x => x.status_code);
                });

            migrationBuilder.CreateTable(
                name: "stockist",
                schema: "inventory",
                columns: table => new
                {
                    stockist_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stockist_name = table.Column<string>(maxLength: 250, nullable: false),
                    stockist_code = table.Column<string>(maxLength: 6, nullable: false),
                    stockist_status = table.Column<string>(maxLength: 25, nullable: true),
                    description = table.Column<string>(maxLength: 500, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stockist", x => x.stockist_id);
                    table.ForeignKey(
                        name: "stockist_stockist_status_fkey",
                        column: x => x.stockist_status,
                        principalSchema: "static",
                        principalTable: "status",
                        principalColumn: "status_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "commission",
                schema: "billing",
                columns: table => new
                {
                    commission_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stockist_id = table.Column<int>(nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "CURRENT_DATE"),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    rate_group = table.Column<int>(nullable: true),
                    inserted_at = table.Column<DateTime>(nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commission", x => x.commission_id);
                    table.ForeignKey(
                        name: "commission_rate_group_fkey",
                        column: x => x.rate_group,
                        principalSchema: "billing",
                        principalTable: "commission_rate_group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "commission_stockist_id_fkey",
                        column: x => x.stockist_id,
                        principalSchema: "inventory",
                        principalTable: "stockist",
                        principalColumn: "stockist_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stockist_detail",
                schema: "inventory",
                columns: table => new
                {
                    stockist_id = table.Column<int>(nullable: false),
                    twitter_handle = table.Column<string>(maxLength: 30, nullable: true),
                    instagram_handle = table.Column<string>(maxLength: 30, nullable: true),
                    facebook_handle = table.Column<string>(maxLength: 30, nullable: true),
                    website_url = table.Column<string>(maxLength: 150, nullable: true),
                    image_url = table.Column<string>(maxLength: 150, nullable: true),
                    tumblr_handle = table.Column<string>(maxLength: 30, nullable: true),
                    email_address = table.Column<string>(maxLength: 100, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("stockist_detail_pkey", x => x.stockist_id);
                    table.ForeignKey(
                        name: "stockist_detail_stockist_id_fkey",
                        column: x => x.stockist_id,
                        principalSchema: "inventory",
                        principalTable: "stockist",
                        principalColumn: "stockist_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_commission_rate_group",
                schema: "billing",
                table: "commission",
                column: "rate_group");

            migrationBuilder.CreateIndex(
                name: "IX_commission_stockist_id",
                schema: "billing",
                table: "commission",
                column: "stockist_id");

            migrationBuilder.CreateIndex(
                name: "IX_stockist_stockist_status",
                schema: "inventory",
                table: "stockist",
                column: "stockist_status");

            migrationBuilder.CreateIndex(
                name: "stockist_stockist_code_key",
                schema: "inventory",
                table: "stockist",
                column: "stockist_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "status_status_code_key",
                schema: "static",
                table: "status",
                column: "status_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "commission",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "stockist_detail",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "commission_rate_group",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "stockist",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "status",
                schema: "static");
        }
    }
}
