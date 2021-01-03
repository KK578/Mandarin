using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Stockists;
using Mandarin.Tests.Data;
using Mandarin.Tests.Extensions;
using Mandarin.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Grpc
{
    [Collection(nameof(MandarinTestsCollectionFixture))]
    public class StockistsGrpcServiceTests : MandarinGrpcIntegrationTestsBase
    {
        public StockistsGrpcServiceTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        private IStockistService Subject => this.Resolve<IStockistService>();

        [Fact]
        public async Task ShouldBeAbleToRetrieveAStockist()
        {
            var stockistCode = WellKnownTestData.Stockists.KelbyTynan.StockistCode;
            var stockist = await this.Subject.GetStockistByCodeAsync(stockistCode);
            stockist.Should().MatchStockist(WellKnownTestData.Stockists.KelbyTynan);
        }

        [Fact]
        public async Task ShouldBeAbleToListAllStockists()
        {
            var stockists = await this.Subject.GetStockistsAsync();
            stockists.Should().HaveCount(10);
        }

        [Fact]
        public async Task ShouldBeAbleToAddANewStockistAndRetrieveDetailsBack()
        {
            await this.Subject.SaveStockistAsync(WellKnownTestData.Stockists.ArlueneWoodes);
            var stockist = await this.Subject.GetStockistByCodeAsync(WellKnownTestData.Stockists.ArlueneWoodes.StockistCode);
            stockist.Should().MatchStockistIgnoringIds(WellKnownTestData.Stockists.ArlueneWoodes);
        }

        [Fact]
        public async Task ShouldBeAbleToUpdateAStockistAndVerifyPersisted()
        {
            var existing = await this.Subject.GetStockistByCodeAsync(WellKnownTestData.Stockists.KelbyTynan.StockistCode);
            existing.Should().MatchStockistIgnoringIds(WellKnownTestData.Stockists.KelbyTynan);

            existing.Details.EmailAddress = TestData.NextString();
            existing.Details.Description = "New Description";

            await this.Subject.SaveStockistAsync(existing);

            var updated = await this.Subject.GetStockistByCodeAsync(WellKnownTestData.Stockists.KelbyTynan.StockistCode);
            updated.Should().MatchStockistIgnoringIds(existing);
        }
    }
}
