using Mandarin.Models.Artists;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mandarin.Database.Migrations
{
    /// <summary>
    /// BASHI-62: Add flag to determine if a stockist should be displayed publically
    ///
    /// Add a flag to <see cref="StockistDetail"/> for determining whether an active artist needs to be displayed
    /// on the public website.
    /// </summary>
    public partial class StockistDetail_AddVisibleFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "visible",
                schema: "inventory",
                table: "stockist_detail",
                nullable: true);

            migrationBuilder.Sql(@"
    UPDATE inventory.stockist_detail d
    SET visible = TRUE
    FROM inventory.stockist s
    WHERE d.stockist_id = s.stockist_id
        AND s.stockist_status = 'ACTIVE'");

            migrationBuilder.Sql(@"
    UPDATE inventory.stockist_detail d
    SET visible = FALSE
    FROM inventory.stockist s
    WHERE d.stockist_id = s.stockist_id
        AND s.stockist_status <> 'ACTIVE'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "visible",
                schema: "inventory",
                table: "stockist_detail");
        }
    }
}
