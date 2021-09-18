using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Commissions;
using Mandarin.Stockists;
using Mandarin.Tests.Data;
using Mandarin.Tests.Data.Extensions;
using Mandarin.Tests.Helpers;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests.Commissions
{
    public sealed class MandarinGrpcRecordOfSalesRepositoryTests : MandarinGrpcIntegrationTestsBase
    {
        private static readonly LocalDate Start = new(2021, 06, 16);
        private static readonly LocalDate End = new(2021, 07, 17);
        private static readonly DateInterval Interval = new(MandarinGrpcRecordOfSalesRepositoryTests.Start, MandarinGrpcRecordOfSalesRepositoryTests.End);

        public MandarinGrpcRecordOfSalesRepositoryTests(MandarinGrpcClientFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        private IRecordOfSalesRepository Subject => this.Resolve<IRecordOfSalesRepository>();

        [Fact]
        public async Task ShouldBeAbleToRetrieveEntriesForRecordsOfSales()
        {
            var recordOfSales = await this.Subject.GetRecordOfSalesAsync(MandarinGrpcRecordOfSalesRepositoryTests.Interval);
            var salesByStockistCode = recordOfSales.ToDictionary(x => StockistCode.Of(x.StockistCode));

            salesByStockistCode[WellKnownTestData.Stockists.KelbyTynan.StockistCode].Should().MatchRecordOfSales(WellKnownTestData.RecordsOfSales.KelbyTynan);
            salesByStockistCode[WellKnownTestData.Stockists.OthilieMapples.StockistCode].Should().MatchRecordOfSales(WellKnownTestData.RecordsOfSales.OthilieMapples);
        }
    }
}
