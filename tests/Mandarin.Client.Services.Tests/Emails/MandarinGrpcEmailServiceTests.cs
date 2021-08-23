using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Emails;
using Mandarin.Tests.Data;
using Mandarin.Tests.Helpers;
using Mandarin.Tests.Helpers.SendGrid;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests.Emails
{
    [Collection(nameof(MandarinClientServicesTestsCollectionFixture))]
    public class MandarinGrpcEmailServiceTests : MandarinGrpcIntegrationTestsBase, IClassFixture<SendGridWireMockFixture>
    {
        public MandarinGrpcEmailServiceTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        private IEmailService Subject => this.Resolve<IEmailService>();

        [Fact]
        public async Task ShouldBeAbleToSuccessfullySendAnEmail()
        {
            var recordOfSales = WellKnownTestData.RecordsOfSales.KelbyTynan;
            var response = await this.Subject.SendRecordOfSalesEmailAsync(recordOfSales);
            response.IsSuccess.Should().BeTrue();
        }
    }
}
