using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Tests.Helpers.Database;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Helpers
{
    [Collection(nameof(MandarinTestsCollectionFixture))]
    public abstract class MandarinIntegrationTestsBase : IAsyncLifetime
    {
        protected MandarinIntegrationTestsBase(MandarinServerFixture fixture, ITestOutputHelper testOutputHelper)
        {
            this.Fixture = fixture;
            this.Fixture.TestOutputHelper = testOutputHelper;
        }

        protected MandarinServerFixture Fixture { get; }

        public async Task InitializeAsync()
        {
            using var scope = this.Fixture.Services.CreateScope();
            var mandarinDbContext = scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            await mandarinDbContext.SeedTestDataAsync();
        }

        public async Task DisposeAsync()
        {
            using var scope = this.Fixture.Services.CreateScope();
            var mandarinDbContext = scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            await mandarinDbContext.CleanupTestDataAsync();
        }
    }
}
