using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Tests.Helpers.Database;
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
        }

        public async Task CleanDatabaseAsync()
        {
            using var scope = this.Services.CreateScope();
            var mandarinDbContext = scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            await mandarinDbContext.CleanupTestDataAsync();
        }
    }
}
