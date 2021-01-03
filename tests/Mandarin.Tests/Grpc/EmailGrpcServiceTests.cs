using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Models.Commissions;
using Mandarin.Services;
using Mandarin.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Grpc
{
    [Collection(nameof(MandarinTestsCollectionFixture))]
    public class EmailGrpcServiceTests : MandarinGrpcIntegrationTestsBase
    {
        public EmailGrpcServiceTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        private IEmailService Subject => this.Resolve<IEmailService>();

        [Fact]
        public async Task ShouldBeAbleToSuccessfullySendAnEmail()
        {
            this.GivenSendGridReturns(new Response(HttpStatusCode.Accepted, null, null));
            var recordOfSales = TestData.Create<RecordOfSales>();
            var response = await this.Subject.SendRecordOfSalesEmailAsync(recordOfSales);
            response.IsSuccess.Should().BeTrue();
        }

        private void GivenSendGridReturns(Response response)
        {
            var sendGridClient = this.Fixture.Services.GetRequiredService<Mock<ISendGridClient>>();
            sendGridClient.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(response);
        }
    }
}
