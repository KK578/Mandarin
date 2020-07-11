using System.Linq;
using Bashi.Core.Enums;
using Mandarin.Models.Common;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Mandarin.Database.Migrations
{
    /// <summary>
    /// BASHI-62: Add Method to Determine if a Stockist should be Displayed
    ///
    /// Extend the Status field to allow for Active and ActiveHidden to add undisplayed stockists.
    /// </summary>
    public partial class ChangeStatusTableToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "stockist_stockist_status_fkey",
                schema: "inventory",
                table: "stockist");

            migrationBuilder.DropIndex(
                name: "IX_stockist_stockist_status",
                schema: "inventory",
                table: "stockist");

            migrationBuilder.DropTable(
                name: "status",
                schema: "static");

            migrationBuilder.AlterColumn<string>(
                name: "stockist_status",
                schema: "inventory",
                table: "stockist",
                type: "character varying(25)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "static");

            migrationBuilder.AlterColumn<string>(
                name: "stockist_status",
                schema: "inventory",
                table: "stockist",
                type: "character varying(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(25)");

            migrationBuilder.CreateTable(
                name: "status",
                schema: "static",
                columns: table => new
                {
                    status_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status_code = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status", x => x.status_id);
                    table.UniqueConstraint("AK_status_status_code", x => x.status_code);
                });

            var statuses = EnumUtil.GetValues<StatusMode>().ToList();
            foreach (var status in statuses)
            {
                migrationBuilder.InsertData(table: "status",
                                            schema: "static",
                                            columns: new[] { "status_code", "description" },
                                            values: new object[] { status.ToString(), status.GetDescription() });
            }

            migrationBuilder.CreateIndex(
                name: "IX_stockist_stockist_status",
                schema: "inventory",
                table: "stockist",
                column: "stockist_status");

            migrationBuilder.CreateIndex(
                name: "status_status_code_key",
                schema: "static",
                table: "status",
                column: "status_code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "stockist_stockist_status_fkey",
                schema: "inventory",
                table: "stockist",
                column: "stockist_status",
                principalSchema: "static",
                principalTable: "status",
                principalColumn: "status_code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
