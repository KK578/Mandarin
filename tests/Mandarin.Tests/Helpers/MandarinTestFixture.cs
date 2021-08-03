using System;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Tests.Helpers.Database;
using Mandarin.Transactions.External;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Npgsql;
using SendGrid;

namespace Mandarin.Tests.Helpers
{
    public class MandarinTestFixture : MandarinApplicationFactory
    {
        public async Task SeedDatabaseAsync()
        {
            using var scope = this.Services.CreateScope();
            var mandarinDbContext = scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            await mandarinDbContext.SeedTestDataAsync();
            await MandarinTestFixture.RefreshConnectionAfterMigrationAsync(mandarinDbContext);
            await MandarinTestFixture.SynchronizeTransactions(scope.ServiceProvider.GetRequiredService<ITransactionSynchronizer>());
        }

        public async Task CleanDatabaseAsync()
        {
            using var scope = this.Services.CreateScope();
            var mandarinDbContext = scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            await mandarinDbContext.CleanupTestDataAsync();
        }

        protected override void ConfigureTestServices(IServiceCollection services)
        {
            base.ConfigureTestServices(services);
            services.AddSingleton<Mock<ISendGridClient>>();
            services.AddTransient(s => s.GetRequiredService<Mock<ISendGridClient>>().Object);
        }

        private static async Task RefreshConnectionAfterMigrationAsync(MandarinDbContext mandarinDbContext)
        {
            using var connection = mandarinDbContext.GetConnection();
            if (connection is not NpgsqlConnection npgsqlConnection)
            {
                throw new InvalidOperationException("Cannot perform post-migration steps as database connection was not PostgreSQL.");
            }

            await npgsqlConnection.OpenAsync();
            npgsqlConnection.ReloadTypes();
        }

        private static async Task SynchronizeTransactions(ITransactionSynchronizer transactionSynchronizer)
        {
            await transactionSynchronizer.SynchronizeTransactionAsync(new ExternalTransactionId("sNVseFoHwzywEiVV69mNfK5eV"));
        }
    }
}
