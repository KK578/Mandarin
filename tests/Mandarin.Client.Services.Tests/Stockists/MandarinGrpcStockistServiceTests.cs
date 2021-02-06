using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Client.Services.Tests.Extensions;
using Mandarin.Stockists;
using Mandarin.Tests.Data;
using Mandarin.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests.Stockists
{
    [Collection(nameof(MandarinClientServicesTestsCollectionFixture))]
    public class MandarinGrpcStockistServiceTests : MandarinGrpcIntegrationTestsBase
    {
        public MandarinGrpcStockistServiceTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        private IStockistService Subject => this.Resolve<IStockistService>();

        [Fact]
        public async Task ShouldBeAbleToRetrieveWellKnownStockists()
        {
            await VerifyStockist(WellKnownTestData.Stockists.KelbyTynan);
            await VerifyStockist(WellKnownTestData.Stockists.OthilieMapples);

            async Task VerifyStockist(Stockist expected)
            {
                var actual = await this.Subject.GetStockistByCodeAsync(expected.StockistCode);
                actual.Should().MatchStockist(expected);
            }
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
            existing.Details.FirstName = "New Name";

            await this.Subject.SaveStockistAsync(existing);

            var updated = await this.Subject.GetStockistByCodeAsync(WellKnownTestData.Stockists.KelbyTynan.StockistCode);
            updated.Should().MatchStockistIgnoringIds(existing);
        }
    }
}
