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
            await connection.ExecuteAsync(@"TRUNCATE TABLE billing.commission_rate_group, billing.commission, inventory.stockist_detail, inventory.stockist");
            await connection.ExecuteAsync(@"TRUNCATE TABLE public.schemaversions");
        }
    }
}
