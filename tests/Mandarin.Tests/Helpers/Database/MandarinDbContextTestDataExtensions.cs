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
            await connection.ExecuteAsync("DROP TABLE billing.commission");
            await connection.ExecuteAsync("DROP TABLE inventory.stockist_detail");
            await connection.ExecuteAsync("DROP TABLE inventory.stockist");
            await connection.ExecuteAsync("TRUNCATE TABLE public.schemaversions");
        }
    }
}
