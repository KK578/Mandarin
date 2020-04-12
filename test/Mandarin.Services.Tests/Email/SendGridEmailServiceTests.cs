using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlazorInputFile;
using Mandarin.Models.Contact;
using Mandarin.Services.Email;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services.Tests.Email
{
    [TestFixture]
    public class SendGridEmailServiceTests
    {
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

            var subject = new SendGridEmailService(Mock.Of<ISendGridClient>(), NullLogger<SendGridEmailService>.Instance);

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

            var subject = new SendGridEmailService(Mock.Of<ISendGridClient>(), NullLogger<SendGridEmailService>.Instance);
            var result = await subject.BuildEmailAsync(model);

            Assert.That(result.From.Email, Is.EqualTo("MyValid@Email.com"));
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

            var subject = new SendGridEmailService(Mock.Of<ISendGridClient>(), NullLogger<SendGridEmailService>.Instance);
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
                    Mock.Of<IFileListEntry>(x => x.Name == "File.zip" && x.Size == 1024 && x.Data == data && x.Type == "zip")
                }
            };

            var subject = new SendGridEmailService(Mock.Of<ISendGridClient>(), NullLogger<SendGridEmailService>.Instance);
            var result = await subject.BuildEmailAsync(model);

            Assert.That(result.Attachments.Count, Is.EqualTo(1));
            Assert.That(Convert.FromBase64String(result.Attachments[0].Content), Is.EqualTo("Attachment"));
        }

        [Test]
        public async Task SendEmailAsync_GivenResponse_StatusCodeIsCopiedToResponse()
        {
            var email = new SendGridMessage();
            var sendGridClient = new Mock<ISendGridClient>();
            sendGridClient.Setup(x => x.SendEmailAsync(email, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new Response(HttpStatusCode.Accepted, new StringContent(""), null));
            var subject = new SendGridEmailService(sendGridClient.Object, NullLogger<SendGridEmailService>.Instance);
            var result = await subject.SendEmailAsync(email);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(202));
        }
    }
}
