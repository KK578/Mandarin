using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Emails;
using Mandarin.Tests.Data;
using Mandarin.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests.Emails
{
    public sealed class MandarinGrpcEmailServiceTests : MandarinGrpcIntegrationTestsBase
    {
        public MandarinGrpcEmailServiceTests(MandarinServerFixture fixture, ITestOutputHelper testOutputHelper)
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
