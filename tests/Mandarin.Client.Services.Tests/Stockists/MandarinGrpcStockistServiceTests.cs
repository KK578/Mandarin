using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Stockists;
using Mandarin.Tests.Data;
using Mandarin.Tests.Data.Extensions;
using Mandarin.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests.Stockists
{
    public sealed class MandarinGrpcStockistServiceTests : MandarinGrpcIntegrationTestsBase
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
                actual.Should().BeEquivalentTo(expected);
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

            var updated = existing with
            {
                Details = existing.Details with
                {
                    EmailAddress = MandarinFixture.Instance.NewString,
                    FirstName = "New Name",
                },
            };

            await this.Subject.SaveStockistAsync(updated);

            var result = await this.Subject.GetStockistByCodeAsync(WellKnownTestData.Stockists.KelbyTynan.StockistCode);
            result.Should().MatchStockistIgnoringIds(updated);
        }
    }
}
