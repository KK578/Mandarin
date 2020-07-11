using Microsoft.EntityFrameworkCore.Migrations;

namespace Mandarin.Database.Migrations
{
    /// <summary>
    /// BASHI-66: Update Artist Page Design
    ///
    /// Update fields to include new display fields for the new grid format.
    /// </summary>
    public partial class AddStockistDisplayFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "inventory",
                table: "stockist_detail",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "full_display_name",
                schema: "inventory",
                table: "stockist_detail",
                maxLength: 250,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "short_display_name",
                schema: "inventory",
                table: "stockist_detail",
                maxLength: 250,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "thumbnail_image_url",
                schema: "inventory",
                table: "stockist_detail",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                schema: "inventory",
                table: "stockist",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                schema: "inventory",
                table: "stockist",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql(@"
    UPDATE inventory.stockist_detail detail
    SET description = s.description,
        short_display_name = s.stockist_name,
        full_display_name = s.stockist_name
    FROM inventory.stockist s WHERE detail.stockist_id = s.stockist_id;
");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "inventory",
                table: "stockist");

            migrationBuilder.DropColumn(
                name: "stockist_name",
                schema: "inventory",
                table: "stockist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "inventory",
                table: "stockist",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "stockist_name",
                schema: "inventory",
                table: "stockist",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.Sql(@"
    UPDATE inventory.stockist s
    SET description = d.description,
        stockist_name = d.full_display_name
    FROM inventory.stockist_detail d WHERE d.stockist_id = s.stockist_id;
");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "inventory",
                table: "stockist_detail");

            migrationBuilder.DropColumn(
                name: "full_display_name",
                schema: "inventory",
                table: "stockist_detail");

            migrationBuilder.DropColumn(
                name: "short_display_name",
                schema: "inventory",
                table: "stockist_detail");

            migrationBuilder.DropColumn(
                name: "thumbnail_image_url",
                schema: "inventory",
                table: "stockist_detail");

            migrationBuilder.DropColumn(
                name: "first_name",
                schema: "inventory",
                table: "stockist");

            migrationBuilder.DropColumn(
                name: "last_name",
                schema: "inventory",
                table: "stockist");
        }
    }
}
