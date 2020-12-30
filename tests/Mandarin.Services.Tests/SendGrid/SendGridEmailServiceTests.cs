using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Bashi.Tests.Framework.Logging;
using FluentAssertions;
using Mandarin.Models.Commissions;
using Mandarin.Services.SendGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services.Tests.SendGrid
{
    [TestFixture]
    public class SendGridEmailServiceTests
    {
        private const string ServiceEmail = "noreply@example.com";
        private const string RealContactEmail = "contact@example.com";
        private const string TemplateId = "TemplateId";

        private Mock<ISendGridClient> sendGridClient;
        private IOptions<SendGridConfiguration> configuration;
        private TestableLogger<SendGridEmailService> logger;

        private IEmailService Subject => new SendGridEmailService(this.sendGridClient.Object, this.configuration, this.logger);

        [SetUp]
        public void SetUp()
        {
            this.sendGridClient = new Mock<ISendGridClient>();
            this.configuration = Options.Create(new SendGridConfiguration
            {
                ServiceEmail = SendGridEmailServiceTests.ServiceEmail,
                RealContactEmail = SendGridEmailServiceTests.RealContactEmail,
                RecordOfSalesTemplateId = SendGridEmailServiceTests.TemplateId,
            });
            this.logger = new TestableLogger<SendGridEmailService>();
        }

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

        [TestFixture]
        private class SendRecordOfSalesEmailAsyncTests : SendGridEmailServiceTests
        {
            [Test]
            public async Task ShouldCopyEmailDetailsFromConfiguration()
            {
                var model = TestData.Create<RecordOfSales>();

                this.GivenExpectedEmailMatches(result =>
                {
                    Assert.That(result.From.Email, Is.EqualTo(SendGridEmailServiceTests.ServiceEmail));
                    Assert.That(result.ReplyTo.Email, Is.EqualTo(SendGridEmailServiceTests.RealContactEmail));
                    Assert.That(result.Personalizations[0].Bccs[0].Email, Is.EqualTo(SendGridEmailServiceTests.RealContactEmail));
                    Assert.That(result.Personalizations[0].Tos[0].Email, Is.EqualTo(model.EmailAddress));
                    Assert.That(result.TemplateId, Is.EqualTo(SendGridEmailServiceTests.TemplateId));
                    result.Personalizations[0].TemplateData
                          .Should().BeEquivalentTo(model, o => o.Excluding(a => a.EmailAddress)
                                                                .Excluding(sales => sales.CustomMessage));
                });

                var response = await this.Subject.SendRecordOfSalesEmailAsync(model);
                Assert.That(response.IsSuccess, Is.True);
            }

            [Test]
            public async Task ShouldNotSetBccIfEmailIsSentToRealContactEmail()
            {
                var model = TestData.Create<RecordOfSales>();
                model = model.WithMessageCustomisations(SendGridEmailServiceTests.RealContactEmail, model.CustomMessage);

                this.GivenExpectedEmailMatches(result =>
                {
                    Assert.That(result.Personalizations[0].Bccs, Is.Null);
                });

                var response = await this.Subject.SendRecordOfSalesEmailAsync(model);
                Assert.That(response.IsSuccess, Is.True);
            }

            [Test]
            public async Task ShouldLogAndRespondWithErrorIfUnsuccessfulOnSendingEmail()
            {
                var model = TestData.Create<RecordOfSales>();
                var sendGridResponse = new Response(HttpStatusCode.Unauthorized, new StringContent("Invalid API Key"), null);
                this.GivenSendGridReturns(sendGridResponse);

                var response = await this.Subject.SendRecordOfSalesEmailAsync(model);

                Assert.That(response.IsSuccess, Is.False);
                Assert.That(response.StatusCode, Is.EqualTo(401));
                Assert.That(this.logger.LogEntries.Count, Is.EqualTo(1));
                Assert.That(this.logger.LogEntries[0].LogLevel, Is.EqualTo(LogLevel.Information));
                Assert.That(this.logger.LogEntries[0].Message, Contains.Substring("Invalid API Key"));
            }

            [Test]
            public void SendEmailAsync_GivenResponseBodyIsNull_NoExceptionIsThrown()
            {
                var model = TestData.Create<RecordOfSales>();
                var sendGridResponse = new Response(HttpStatusCode.Accepted, null, null);
                this.GivenSendGridReturns(sendGridResponse);

                Assert.DoesNotThrowAsync(() => this.Subject.SendRecordOfSalesEmailAsync(model));
            }

            [Test]
            public async Task SendEmailAsync_GivenResponse_StatusCodeIsCopiedToResponse()
            {
                var model = TestData.Create<RecordOfSales>();
                var sendGridResponse = new Response(HttpStatusCode.Accepted, new StringContent(string.Empty), null);
                this.GivenSendGridReturns(sendGridResponse);

                var response = await this.Subject.SendRecordOfSalesEmailAsync(model);

                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.StatusCode, Is.EqualTo(202));
            }
        }
    }
}
