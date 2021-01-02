using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Services;
using Mandarin.Tests.Data;
using Mandarin.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Grpc.Stockists
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
            stockist.Should().BeEquivalentTo(WellKnownTestData.Stockists.KelbyTynan, o => o.IgnoringCyclicReferences());
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
            stockist.Should().BeEquivalentTo(WellKnownTestData.Stockists.ArlueneWoodes,
                                             o => o.IgnoringCyclicReferences()
                                                   .Excluding(x => x.StockistId)
                                                   .Excluding(x => x.Details.StockistId));
        }
    }
}
