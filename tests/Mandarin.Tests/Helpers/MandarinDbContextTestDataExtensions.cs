using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Models.Stockists;
using Mandarin.Tests.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandarin.Tests.Helpers
{
    internal static class MandarinDbContextTestDataExtensions
    {
        public static async Task SeedTestDataAsync(this MandarinDbContext mandarinDbContext)
        {
            await mandarinDbContext.AddStockistIfNotPresentAsync(WellKnownTestData.Stockists.TheLittleMandarinStockist);

            await mandarinDbContext.SaveChangesAsync();
        }

        public static async Task CleanupTestDataAsync(this MandarinDbContext mandarinDbContext)
        {
            await mandarinDbContext.Database.ExecuteSqlRawAsync(@"TRUNCATE TABLE billing.commission_rate_group, billing.commission, inventory.stockist_detail, inventory.stockist RESTART IDENTITY");
        }

        private static async Task AddStockistIfNotPresentAsync(this MandarinDbContext mandarinDbContext, Stockist stockist)
        {
            if (!await mandarinDbContext.Stockist.AnyAsync(x => x.StockistCode == stockist.StockistCode))
            {
                await mandarinDbContext.Stockist.AddAsync(stockist);
            }
        }
    }
}
