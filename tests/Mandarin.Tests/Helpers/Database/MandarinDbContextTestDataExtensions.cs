using System.Data;
using System.Threading.Tasks;
using Dapper;
using Mandarin.Database;
using Mandarin.Models.Stockists;
using Mandarin.Tests.Data;

namespace Mandarin.Tests.Helpers.Database
{
    internal static class MandarinDbContextTestDataExtensions
    {
        public static async Task SeedTestDataAsync(this MandarinDbContext mandarinDbContext)
        {
            using var connection = mandarinDbContext.GetConnection();
            connection.Open();
            await connection.AddStockistIfNotPresentAsync(WellKnownTestData.Stockists.TheLittleMandarinStockist);
            await connection.ExecuteAsync(@"ALTER SEQUENCE inventory.stockist_stockist_id_seq RESTART WITH 5;");
        }

        public static async Task CleanupTestDataAsync(this MandarinDbContext mandarinDbContext)
        {
            using var connection = mandarinDbContext.GetConnection();
            connection.Open();
            await connection.ExecuteAsync(@"TRUNCATE TABLE billing.commission_rate_group, billing.commission, inventory.stockist_detail, inventory.stockist");
        }

        private static async Task AddStockistIfNotPresentAsync(this IDbConnection connection, Stockist stockist)
        {
            var existing = await connection.QueryFirstOrDefaultAsync("SELECT * FROM inventory.stockist WHERE stockist_code = @StockistCode", new { StockistCode = stockist.StockistCode });
            if (existing == null)
            {
                var sql = @"INSERT INTO inventory.stockist (stockist_code, stockist_status, first_name, last_name)
                            VALUES (@StockistCode, @StatusCode, @FirstName, @LastName)";

                await connection.ExecuteAsync(sql, stockist);
            }
        }
    }
}
