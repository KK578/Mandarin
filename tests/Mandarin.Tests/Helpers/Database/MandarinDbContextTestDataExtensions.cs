using System.Threading.Tasks;
using Dapper;
using Mandarin.Database;

namespace Mandarin.Tests.Helpers.Database
{
    internal static class MandarinDbContextTestDataExtensions
    {
        public static Task SeedTestDataAsync(this MandarinDbContext mandarinDbContext)
        {
            using var connection = mandarinDbContext.GetConnection();
            connection.Open();
            mandarinDbContext.RunMigrations();
            return Task.CompletedTask;
        }

        public static async Task CleanupTestDataAsync(this MandarinDbContext mandarinDbContext)
        {
            using var connection = mandarinDbContext.GetConnection();
            connection.Open();
            await connection.ExecuteAsync("DROP TABLE IF EXISTS billing.commission");
            await connection.ExecuteAsync("DROP TABLE IF EXISTS inventory.stockist_detail");
            await connection.ExecuteAsync("DROP TABLE IF EXISTS inventory.stockist");
            await connection.ExecuteAsync("DROP TABLE IF EXISTS inventory.fixed_commission_amount");
            await connection.ExecuteAsync("DROP TABLE IF EXISTS inventory.frame_price");
            await connection.ExecuteAsync("DROP TABLE IF EXISTS inventory.product");
            await connection.ExecuteAsync("DROP PROCEDURE IF EXISTS inventory.sp_frame_price_upsert(TEXT, NUMERIC, TIMESTAMP)");
            await connection.ExecuteAsync("TRUNCATE TABLE public.schemaversions");
        }
    }
}
