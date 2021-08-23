using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Emails;
using Mandarin.Services.Emails;
using Mandarin.Tests.Data;
using Microsoft.Extensions.Options;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;
using Xunit;

namespace Mandarin.Services.Tests.SendGrid
{
    public class SendGridEmailServiceTests
    {
        private const string ServiceEmail = "noreply@example.com";
        private const string RealContactEmail = "contact@example.com";
        private const string TemplateId = "TemplateId";

        private readonly Mock<ISendGridClient> sendGridClient;
        private readonly IOptions<SendGridConfiguration> configuration;

        protected SendGridEmailServiceTests()
        {
            this.sendGridClient = new Mock<ISendGridClient>();
            this.configuration = Options.Create(new SendGridConfiguration
            {
                ServiceEmail = SendGridEmailServiceTests.ServiceEmail,
                RealContactEmail = SendGridEmailServiceTests.RealContactEmail,
                RecordOfSalesTemplateId = SendGridEmailServiceTests.TemplateId,
            });
        }

        private IEmailService Subject => new SendGridEmailService(this.sendGridClient.Object, this.configuration);

        private void GivenExpectedEmailMatches(Action<SendGridMessage> verifyMessageFunc)
        {
            this.sendGridClient.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                .Callback((SendGridMessage message, CancellationToken _) => verifyMessageFunc(message))
                .ReturnsAsync(new Response(HttpStatusCode.Accepted, new StringContent(string.Empty), null));
        }

        private void GivenSendGridReturns(Response response)
        {
            this.sendGridClient.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
        }

        private void GivenSendGridThrows(Exception ex)
        {
            this.sendGridClient.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(ex);
        }

        public class SendRecordOfSalesEmailAsyncTests : SendGridEmailServiceTests
        {
            [Fact]
            public async Task ShouldCopyEmailDetailsFromConfiguration()
            {
                var model = MandarinFixture.Instance.NewRecordOfSales;

                this.GivenExpectedEmailMatches(result =>
                {
                    result.From.Email.Should().Be(SendGridEmailServiceTests.ServiceEmail);
                    result.ReplyTo.Email.Should().Be(SendGridEmailServiceTests.RealContactEmail);
                    result.Personalizations[0].Bccs[0].Email.Should().Be(SendGridEmailServiceTests.RealContactEmail);
                    result.Personalizations[0].Tos[0].Email.Should().Be(model.EmailAddress);
                    result.TemplateId.Should().Be(SendGridEmailServiceTests.TemplateId);
                    result.Personalizations[0].TemplateData.Should()
                          .BeEquivalentTo(model, o => o.Excluding(a => a.EmailAddress)
                                                       .Excluding(sales => sales.CustomMessage));
                });

                var response = await this.Subject.SendRecordOfSalesEmailAsync(model);
                response.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldNotSetBccIfEmailIsSentToRealContactEmail()
            {
                var model = MandarinFixture.Instance.NewRecordOfSales;
                model = model.WithMessageCustomisations(SendGridEmailServiceTests.RealContactEmail, model.CustomMessage);

                this.GivenExpectedEmailMatches(result => result.Personalizations[0].Bccs.Should().BeNull());

                var response = await this.Subject.SendRecordOfSalesEmailAsync(model);
                response.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldRespondWithErrorIfUnsuccessfulOnSendingEmail()
            {
                var model = MandarinFixture.Instance.NewRecordOfSales;
                var sendGridResponse = new Response(HttpStatusCode.Unauthorized, new StringContent("Invalid API Key"), null);
                this.GivenSendGridReturns(sendGridResponse);

                var response = await this.Subject.SendRecordOfSalesEmailAsync(model);

                response.IsSuccess.Should().BeFalse();
                response.Message.Should().Contain("Invalid API Key");
            }

            [Fact]
            public async Task ShouldCorrectlyProcessMessageWhenSendGridResponseBodyIsEmpty()
            {
                var model = MandarinFixture.Instance.NewRecordOfSales;
                var sendGridResponse = new Response(HttpStatusCode.Accepted, null, null);
                this.GivenSendGridReturns(sendGridResponse);

                var response = await this.Subject.SendRecordOfSalesEmailAsync(model);

                response.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldHaveAnyExceptionMessageInResponseMessage()
            {
                var model = MandarinFixture.Instance.NewRecordOfSales;
                var ex = new Exception("Service didn't work.");
                this.GivenSendGridThrows(ex);
                var response = await this.Subject.SendRecordOfSalesEmailAsync(model);

                response.IsSuccess.Should().BeFalse();
                response.Message.Should().Contain("Service didn't work.");
            }

            [Fact]
            public async Task ShouldShowSuccessIfSendGridAcceptsEmail()
            {
                var model = MandarinFixture.Instance.NewRecordOfSales;
                var sendGridResponse = new Response(HttpStatusCode.Accepted, new StringContent(string.Empty), null);
                this.GivenSendGridReturns(sendGridResponse);

                var response = await this.Subject.SendRecordOfSalesEmailAsync(model);

                response.IsSuccess.Should().BeTrue();
                response.Message.Should().Contain("Success");
            }
        }
    }
}
