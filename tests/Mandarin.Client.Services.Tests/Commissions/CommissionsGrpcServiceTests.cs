using System;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Commissions;
using Mandarin.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests.Commissions
{
    [Collection(nameof(MandarinClientServicesTestsCollectionFixture))]
    public class CommissionsGrpcServiceTests : MandarinGrpcIntegrationTestsBase
    {
        public CommissionsGrpcServiceTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        private ICommissionService Subject => this.Resolve<ICommissionService>();

        [Fact]
        public async Task ShouldBeAbleToRetrieveEntriesForRecordsOfSales()
        {
            // TODO: Assertions required. This test only verifies a round-trip via Sandbox.
            var recordOfSales = await this.Subject.GetRecordOfSalesForPeriodAsync(new DateTime(2020, 06, 16), new DateTime(2020, 07, 17));
            recordOfSales.Should().HaveCount(5);
        }
    }
}
