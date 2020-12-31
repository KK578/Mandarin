using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Mandarin.Client.Services.Tests.Helpers;
using Mandarin.Models.Commissions;
using Mandarin.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Mandarin.Client.Services.Tests.Emails
{
    [TestFixture]
    public class EmailGrpcServiceTests : GrpcServiceTestsBase
    {
        private readonly Mock<ISendGridClient> sendGridClient = new();

        private IEmailService Subject => this.Resolve<IEmailService>();

        [Test]
        public async Task ShouldBeAbleToSuccessfullySendAnEmail()
        {
            this.GivenSendGridReturns(new Response(HttpStatusCode.Accepted, null, null));
            var recordOfSales = TestData.Create<RecordOfSales>();
            var response = await this.Subject.SendRecordOfSalesEmailAsync(recordOfSales);
            Assert.That(response.IsSuccess, Is.True);
        }

        protected override void ConfigureTestServices(IServiceCollection services)
        {
            services.AddSingleton(this.sendGridClient.Object);
        }

        private void GivenSendGridReturns(Response response)
        {
            this.sendGridClient.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
        }
    }
}
