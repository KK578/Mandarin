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
                DROP PROCEDURE inventory.sp_frame_price_upsert(TEXT, NUMERIC, TIMESTAMP WITH TIME ZONE);
                DROP PROCEDURE inventory.sp_product_upsert(VARCHAR, VARCHAR, VARCHAR, VARCHAR, NUMERIC, TIMESTAMP WITH TIME ZONE);
                DROP PROCEDURE billing.sp_transaction_upsert(VARCHAR, NUMERIC, TIMESTAMP WITH TIME ZONE, billing.tvp_subtransaction[]);

                DROP TYPE billing.tvp_subtransaction;

                DROP TABLE billing.commission;
                DROP TABLE billing.subtransaction;
                DROP TABLE billing.transaction;
                DROP TABLE billing.external_transaction;
                DROP TABLE inventory.frame_price;
                DROP TABLE inventory.product;
                DROP TABLE inventory.stockist_detail;
                DROP TABLE inventory.stockist;
                TRUNCATE TABLE public.schemaversions");
        }
    }
}
