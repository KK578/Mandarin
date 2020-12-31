using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Tests.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandarin.Tests.Helpers
{
    internal static class MandarinDbContextTestDataExtensions
    {
        public static async Task SeedTestDataAsync(this MandarinDbContext mandarinDbContext)
        {
            await WellKnownTestData.SeedDatabaseAsync(mandarinDbContext);
            await mandarinDbContext.SaveChangesAsync();
        }

        public static async Task CleanupTestDataAsync(this MandarinDbContext mandarinDbContext)
        {
            await mandarinDbContext.Database.ExecuteSqlRawAsync(@"TRUNCATE TABLE billing.commission_rate_group, billing.commission, inventory.stockist_detail, inventory.stockist RESTART IDENTITY");
        }
    }
}
