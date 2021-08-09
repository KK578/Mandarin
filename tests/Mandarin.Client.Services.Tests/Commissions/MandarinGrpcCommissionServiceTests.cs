using System.Collections.Generic;
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
    [Collection(nameof(MandarinClientServicesTestsCollectionFixture))]
    public class MandarinGrpcCommissionServiceTests : MandarinGrpcIntegrationTestsBase
    {
        private static readonly Instant Start = Instant.FromUtc(2021, 06, 16, 00, 00, 00);
        private static readonly Instant End = Instant.FromUtc(2021, 07, 17, 00, 00, 00);

        private readonly RecordOfSales kelbyTynanRecordOfSales;

        public MandarinGrpcCommissionServiceTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
            this.kelbyTynanRecordOfSales = new RecordOfSales
            {
                StockistCode = WellKnownTestData.Stockists.KelbyTynan.StockistCode.ToString(),
                FirstName = WellKnownTestData.Stockists.KelbyTynan.Details.FirstName,
                Name = WellKnownTestData.Stockists.KelbyTynan.Details.DisplayName,
                EmailAddress = WellKnownTestData.Stockists.KelbyTynan.Details.EmailAddress,
                CustomMessage = string.Empty,
                StartDate = new LocalDate(2021, 06, 16),
                EndDate = new LocalDate(2021, 07, 17),
                Rate = decimal.Divide(WellKnownTestData.Stockists.KelbyTynan.Commission.Rate, 100),
                Sales = new List<Sale>
                {
                    new()
                    {
                        ProductCode = WellKnownTestData.Products.Clementine.ProductCode.ToString(),
                        ProductName = WellKnownTestData.Products.Clementine.ProductName.ToString(),
                        Quantity = 1,
                        UnitPrice = 45.00M,
                        Subtotal = 45.00M,
                        Commission = -4.50M,
                        Total = 40.50M,
                    },
                }.AsReadOnly(),
                Subtotal = 45.00M,
                CommissionTotal = -4.50M,
                Total = 40.50M,
            };
        }

        private ICommissionService Subject => this.Resolve<ICommissionService>();

        [Fact]
        public async Task ShouldBeAbleToRetrieveEntriesForRecordsOfSales()
        {
            var recordOfSales = await this.Subject.GetRecordOfSalesForPeriodAsync(MandarinGrpcCommissionServiceTests.Start, MandarinGrpcCommissionServiceTests.End);
            var salesByStockistCode = recordOfSales.ToDictionary(x => StockistCode.Of(x.StockistCode));

            salesByStockistCode[WellKnownTestData.Stockists.KelbyTynan.StockistCode].Should().MatchRecordOfSales(this.kelbyTynanRecordOfSales);
            salesByStockistCode[WellKnownTestData.Stockists.OthilieMapples.StockistCode].Total.Should().Be(0);
        }
    }
}
