using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Bashi.Tests.Framework.Logging;
using FluentAssertions;
using Mandarin.Models.Commissions;
using Mandarin.Services.SendGrid;
using Microsoft.Extensions.Logging.Abstractions;
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

        private IOptions<SendGridConfiguration> configuration;

        [SetUp]
        public void SetUp()
        {
            this.configuration = Options.Create(new SendGridConfiguration
            {
                ServiceEmail = SendGridEmailServiceTests.ServiceEmail,
                RealContactEmail = SendGridEmailServiceTests.RealContactEmail,
                RecordOfSalesTemplateId = SendGridEmailServiceTests.TemplateId,
            });
        }

        [Test]
        public void BuildRecordOfSalesEmail_GivenValidModel_WhenEmailIsSentToRealContactEmail_ThenBccShouldNotBeSet()
        {
            var model = TestData.Create<ArtistSales>();
            model = model.WithMessageCustomisations(SendGridEmailServiceTests.RealContactEmail, model.CustomMessage);
            var subject = new SendGridEmailService(Mock.Of<ISendGridClient>(), this.configuration, NullLogger<SendGridEmailService>.Instance);
            var result = subject.BuildRecordOfSalesEmail(model);

            Assert.That(result.From.Email, Is.EqualTo(SendGridEmailServiceTests.ServiceEmail));
            Assert.That(result.ReplyTo, Is.Null);
            Assert.That(result.Personalizations[0].Bccs, Is.Null);
            Assert.That(result.Personalizations[0].Tos[0].Email, Is.EqualTo(model.EmailAddress));
            Assert.That(result.TemplateId, Is.EqualTo(SendGridEmailServiceTests.TemplateId));
            result.Personalizations[0].TemplateData
                  .Should().BeEquivalentTo(model, o => o.Excluding(a => a.EmailAddress).Excluding(sales => sales.CustomMessage));
        }

        [Test]
        public void BuildRecordOfSalesEmail_GivenValidModel_ShouldCopyValuesCorrectly()
        {
            var model = TestData.Create<ArtistSales>();
            var subject = new SendGridEmailService(Mock.Of<ISendGridClient>(), this.configuration, NullLogger<SendGridEmailService>.Instance);
            var result = subject.BuildRecordOfSalesEmail(model);

            Assert.That(result.From.Email, Is.EqualTo(SendGridEmailServiceTests.ServiceEmail));
            Assert.That(result.ReplyTo.Email, Is.EqualTo(SendGridEmailServiceTests.RealContactEmail));
            Assert.That(result.Personalizations[0].Bccs[0].Email, Is.EqualTo(SendGridEmailServiceTests.RealContactEmail));
            Assert.That(result.Personalizations[0].Tos[0].Email, Is.EqualTo(model.EmailAddress));
            Assert.That(result.TemplateId, Is.EqualTo(SendGridEmailServiceTests.TemplateId));
            result.Personalizations[0].TemplateData
                  .Should().BeEquivalentTo(model, o => o.Excluding(a => a.EmailAddress).Excluding(sales => sales.CustomMessage));
        }

        [Test]
        public async Task SendEmailAsync_GivenErrorResponse_BodyIsLogged_AndStatusCodeIsCopiedToResponse()
        {
            var email = new SendGridMessage();
            var sendGridClient = new Mock<ISendGridClient>();
            sendGridClient.Setup(x => x.SendEmailAsync(email, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new Response(HttpStatusCode.Unauthorized, new StringContent("Invalid API Key"), null));
            var logger = new TestableLogger<SendGridEmailService>();
            var subject = new SendGridEmailService(sendGridClient.Object, this.configuration, logger);
            var result = await subject.SendEmailAsync(email);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(401));
            Assert.That(logger.LogEntries.Count, Is.EqualTo(1));
            Assert.That(logger.LogEntries[0].Message, Contains.Substring("Invalid API Key"));
        }

        [Test]
        public void SendEmailAsync_GivenResponseBodyIsNull_NoExceptionIsThrown()
        {
            var email = new SendGridMessage();
            var sendGridClient = new Mock<ISendGridClient>();
            sendGridClient.Setup(x => x.SendEmailAsync(email, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new Response(HttpStatusCode.Accepted, null, null));
            var subject = new SendGridEmailService(sendGridClient.Object, this.configuration, NullLogger<SendGridEmailService>.Instance);

            Assert.DoesNotThrowAsync(() => subject.SendEmailAsync(email));
        }

        [Test]
        public async Task SendEmailAsync_GivenResponse_StatusCodeIsCopiedToResponse()
        {
            var email = new SendGridMessage();
            var sendGridClient = new Mock<ISendGridClient>();
            sendGridClient.Setup(x => x.SendEmailAsync(email, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new Response(HttpStatusCode.Accepted, new StringContent(string.Empty), null));
            var subject = new SendGridEmailService(sendGridClient.Object, this.configuration, NullLogger<SendGridEmailService>.Instance);
            var result = await subject.SendEmailAsync(email);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(202));
        }
    }
}
