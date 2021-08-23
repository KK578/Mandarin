using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Tests.Helpers.Database;
using Mandarin.Transactions.External;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.Tests.Helpers
{
    public class MandarinTestFixture : MandarinApplicationFactory
    {
        public async Task SeedDatabaseAsync()
        {
            using var scope = this.Services.CreateScope();
            var mandarinDbContext = scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            await mandarinDbContext.SeedTestDataAsync();
            await MandarinTestFixture.SynchronizeTransactions(scope.ServiceProvider.GetRequiredService<ITransactionSynchronizer>());
        }

        public async Task CleanDatabaseAsync()
        {
            using var scope = this.Services.CreateScope();
            var mandarinDbContext = scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            await mandarinDbContext.CleanupTestDataAsync();
        }

        private static async Task SynchronizeTransactions(ITransactionSynchronizer transactionSynchronizer)
        {
            await transactionSynchronizer.SynchronizeTransactionAsync(ExternalTransactionId.Of("sNVseFoHwzywEiVV69mNfK5eV"));
        }
    }
}
