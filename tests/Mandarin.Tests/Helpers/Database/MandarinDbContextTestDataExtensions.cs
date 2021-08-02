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
            await connection.ExecuteAsync(@"
                DROP PROCEDURE IF EXISTS inventory.sp_frame_price_upsert(TEXT, NUMERIC, TIMESTAMP);
                DROP PROCEDURE IF EXISTS inventory.sp_product_upsert(VARCHAR, VARCHAR, VARCHAR, VARCHAR, NUMERIC, TIMESTAMP);
                DROP PROCEDURE IF EXISTS billing.sp_transaction_upsert(VARCHAR, NUMERIC, TIMESTAMP, billing.TVP_SUBTRANSACTION[]);

                DROP TYPE IF EXISTS billing.tvp_subtransaction;

                DROP TABLE IF EXISTS billing.commission;
                DROP TABLE IF EXISTS billing.external_transaction;
                DROP TABLE IF EXISTS billing.subtransaction;
                DROP TABLE IF EXISTS billing.transaction;
                DROP TABLE IF EXISTS inventory.stockist_detail;
                DROP TABLE IF EXISTS inventory.stockist;
                DROP TABLE IF EXISTS inventory.frame_price;
                DROP TABLE IF EXISTS inventory.product;
                TRUNCATE TABLE public.schemaversions");
        }
    }
}
