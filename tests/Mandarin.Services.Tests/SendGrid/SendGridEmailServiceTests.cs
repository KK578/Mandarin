using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Bashi.Tests.Framework.Logging;
using BlazorInputFile;
using FluentAssertions;
using Mandarin.Models.Commissions;
using Mandarin.Models.Contact;
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
                RecordOfSalesTemplateId = SendGridEmailServiceTests.TemplateId
            });
        }

        [Test]
        public void BuildEmailAsync_GivenAnInvalidModel_ShouldThrowException()
        {
            var model = new ContactDetailsModel()
            {
                Name = "My Name",
                Email = "MyInvalidEmail",
                Reason = ContactReasonType.General,
                Comment = "My extra long comment.",
            };

            var subject = new SendGridEmailService(Mock.Of<ISendGridClient>(), this.configuration, NullLogger<SendGridEmailService>.Instance);

            Assert.That(() => subject.BuildEmailAsync(model), Throws.Exception.InstanceOf<ValidationException>());
        }

        [Test]
        public async Task BuildEmailAsync_GivenValidModel_ShouldCopyValuesCorrectly()
        {
            var model = new ContactDetailsModel
            {
                Name = "My Name",
                Email = "MyValid@Email.com",
                Reason = ContactReasonType.General,
                Comment = "My extra long comment."
            };

            var subject =
                new SendGridEmailService(Mock.Of<ISendGridClient>(), this.configuration, NullLogger<SendGridEmailService>.Instance);
            var result = await subject.BuildEmailAsync(model);

            Assert.That(result.From.Email, Is.EqualTo(SendGridEmailServiceTests.ServiceEmail));
            Assert.That(result.ReplyTo.Email, Is.EqualTo("MyValid@Email.com"));
            Assert.That(result.Personalizations[0].Tos[0].Email, Is.EqualTo(SendGridEmailServiceTests.RealContactEmail));
            Assert.That(result.Subject, Is.EqualTo("My Name - General Query"));
            Assert.That(result.PlainTextContent.Replace("\r\n", "|").Replace("\n", "|"),
                        Is.EqualTo("Reason: General Query||Comment:|My extra long comment.|"));
        }

        [Test]
        public async Task BuildEmailAsync_GivenValidModel_WhenReasonIsOther_ShouldUseAdditionalReason()
        {
            var model = new ContactDetailsModel
            {
                Name = "My Name",
                Email = "MyValid@Email.com",
                Reason = ContactReasonType.Other,
                AdditionalReason = "My Special Reason",
                Comment = "My extra long comment."
            };

            var subject =
                new SendGridEmailService(Mock.Of<ISendGridClient>(), this.configuration, NullLogger<SendGridEmailService>.Instance);
            var result = await subject.BuildEmailAsync(model);

            Assert.That(result.Subject, Is.EqualTo("My Name - Other (My Special Reason)"));
            Assert.That(result.PlainTextContent, Contains.Substring("Reason: My Special Reason"));
        }

        [Test]
        public async Task BuildEmailAsync_GivenAttachment_ShouldCopyDataFromStream()
        {
            var data = new MemoryStream(Encoding.ASCII.GetBytes("Attachment"));
            var model = new ContactDetailsModel
            {
                Name = "My Name",
                Email = "MyValid@Email.com",
                Reason = ContactReasonType.General,
                Comment = "My extra long comment.",
                Attachments = new List<IFileListEntry>
                {
                    Mock.Of<IFileListEntry>(x => x.Name == "File.zip" && x.Size == 1024 && x.Data == data &&
                                                 x.Type == "zip")
                }
            };

            var subject =
                new SendGridEmailService(Mock.Of<ISendGridClient>(), this.configuration, NullLogger<SendGridEmailService>.Instance);
            var result = await subject.BuildEmailAsync(model);

            Assert.That(result.Attachments.Count, Is.EqualTo(1));
            Assert.That(Convert.FromBase64String(result.Attachments[0].Content), Is.EqualTo("Attachment"));
        }

        [Test]
        public void BuildRecordOfSalesEmail_GivenValidModel_ShouldCopyValuesCorrectly()
        {
            var model = TestData.Create<ArtistSales>();
            var subject = new SendGridEmailService(Mock.Of<ISendGridClient>(), this.configuration, NullLogger<SendGridEmailService>.Instance);
            var result = subject.BuildRecordOfSalesEmail(model);

            Assert.That(result.From.Email, Is.EqualTo(SendGridEmailServiceTests.ServiceEmail));
            Assert.That(result.ReplyTo.Email, Is.EqualTo(SendGridEmailServiceTests.RealContactEmail));
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
                          .ReturnsAsync(new Response(HttpStatusCode.Accepted, new StringContent(""), null));
            var subject = new SendGridEmailService(sendGridClient.Object, this.configuration, NullLogger<SendGridEmailService>.Instance);
            var result = await subject.SendEmailAsync(email);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(202));
        }
    }
}
