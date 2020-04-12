using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BlazorInputFile;
using Mandarin.Models.Contact;
using Mandarin.Services.Email;
using Mandarin.ViewModels.Contact;
using Moq;
using NUnit.Framework;
using SendGrid.Helpers.Mail;

namespace Mandarin.ViewModels.Tests.Contact
{
    [TestFixture]
    public class ContactPageViewModelTests
    {
        private Mock<IEmailService> emailService;
        private IContactPageViewModel subject;

        [SetUp]
        public void SetUp()
        {
            this.emailService = new Mock<IEmailService>();
            this.subject = new ContactPageViewModel(this.emailService.Object);
        }

        [Test]
        public async Task OnFileChange_SetsModelAttachments()
        {
            var attachments = new List<IFileListEntry> { Mock.Of<IFileListEntry>() };
            this.subject.OnFileChange(attachments);
            Assert.That(this.subject.Model.Attachments, Is.EqualTo(attachments).AsCollection);
        }

        [Test]
        public async Task SubmitAsync_GivenEmailServiceThrowsException_ThenSubmitExceptionIsSet()
        {
            var exception = new ValidationException("Failed to build email.");
            GivenEmailServiceThrowsOnBuild(exception);
            await WhenContactPageSubmitIsCalled();
            Assert.That(this.subject.SubmitException, Is.EqualTo(exception));
        }

        [Test]
        public async Task SubmitAsync_GivenEmailServiceReturnsInternalServerErrorResponse_ThenLastSubmissionSuccessfulIsFalse()
        {
            GivenEmailServiceReturnsEmailOnBuild();
            GivenEmailServiceReturnsInternalServerErrorOnSend();
            await this.WhenContactPageSubmitIsCalled();
            Assert.That(this.subject.LastSubmitSuccessful, Is.False);
        }

        [Test]
        public async Task SubmitAsync_GivenEmailServiceReturnsOkResponse_ThenLastSubmissionSuccessfulIsTrue()
        {
            GivenEmailServiceReturnsEmailOnBuild();
            GivenEmailServiceReturnsOkOnSend();
            await this.WhenContactPageSubmitIsCalled();
            Assert.That(this.subject.LastSubmitSuccessful, Is.True);
        }


        private void GivenEmailServiceThrowsOnBuild(Exception ex)
        {
            this.emailService.Setup(x => x.BuildEmailAsync(It.IsAny<ContactDetailsModel>())).Throws(ex);
        }

        private void GivenEmailServiceReturnsEmailOnBuild()
        {
            var email = new SendGridMessage();
            this.emailService.Setup(x => x.BuildEmailAsync(It.IsAny<ContactDetailsModel>())).ReturnsAsync(email);
        }

        private void GivenEmailServiceReturnsOkOnSend()
        {
            var response = new EmailResponse(200);
            this.emailService.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>())).ReturnsAsync(response);
        }

        private void GivenEmailServiceReturnsInternalServerErrorOnSend()
        {
            var response = new EmailResponse(500);
            this.emailService.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>())).ReturnsAsync(response);
        }


        private Task WhenContactPageSubmitIsCalled()
        {
            return this.subject.SubmitAsync();
        }

    }
}
