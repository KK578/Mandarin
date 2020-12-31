using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Client.Services.Tests.Helpers;
using Mandarin.Services;
using Mandarin.Tests.Data;
using NUnit.Framework;

namespace Mandarin.Client.Services.Tests.Stockists
{
    [TestFixture]
    public class StockistsGrpcServiceTests : GrpcServiceTestsBase
    {
        private IStockistService Subject => this.Resolve<IStockistService>();

        private class GetStockistByCodeAsyncTests : StockistsGrpcServiceTests
        {
            [Test]
            public async Task ShouldBeAbleToRetrieveAStockist()
            {
                var stockistCode = WellKnownTestData.Stockists.TheLittleMandarinStockist.StockistCode;
                var stockist = await this.Subject.GetStockistByCodeAsync(stockistCode);
                stockist.Should().BeEquivalentTo(WellKnownTestData.Stockists.TheLittleMandarinStockist,
                                                 o => o.IgnoringCyclicReferences());
            }
        }

        private class GetStockistsAsyncTests : StockistsGrpcServiceTests
        {
            [Test]
            public async Task ShouldBeAbleToListAllStockists()
            {
                var stockists = await this.Subject.GetStockistsAsync();
                Assert.That(stockists, Has.Count.EqualTo(1));
            }
        }

        private class SaveStockistsAsyncTests : StockistsGrpcServiceTests
        {
            [Test]
            public async Task ShouldBeAbleToAddANewStockistAndRetrieveDetailsBack()
            {
                await this.Subject.SaveStockistAsync(WellKnownTestData.Stockists.HiddenStockist);
                var stockist = await this.Subject.GetStockistByCodeAsync(WellKnownTestData.Stockists.HiddenStockist.StockistCode);
                stockist.Should().BeEquivalentTo(WellKnownTestData.Stockists.HiddenStockist,
                                                 o => o.IgnoringCyclicReferences()
                                                       .Excluding(x => x.StockistId)
                                                       .Excluding(x => x.Details.StockistId));
            }
        }
    }
}
