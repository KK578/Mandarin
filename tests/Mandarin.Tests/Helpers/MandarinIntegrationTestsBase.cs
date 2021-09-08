using System.Globalization;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Helpers
{
    [Collection(nameof(MandarinTestsCollectionFixture))]
    public abstract class MandarinIntegrationTestsBase : IAsyncLifetime
    {
        protected MandarinIntegrationTestsBase(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-GB");
            fixture.TestOutputHelper = testOutputHelper;
            this.Fixture = fixture;
        }

        public MandarinTestFixture Fixture { get; }

        public async Task InitializeAsync()
        {
            await this.Fixture.SeedDatabaseAsync();
        }

        public async Task DisposeAsync()
        {
            await this.Fixture.CleanDatabaseAsync();
        }
    }
}
