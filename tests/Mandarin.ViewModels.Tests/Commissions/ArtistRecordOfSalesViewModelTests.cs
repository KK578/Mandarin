using System;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Mandarin.Models.Commissions;
using Mandarin.Services;
using Mandarin.Services.Objects;
using Mandarin.ViewModels.Commissions;
using Moq;
using NUnit.Framework;
using SendGrid.Helpers.Mail;

namespace Mandarin.ViewModels.Tests.Commissions
{
    [TestFixture]
    public class ArtistRecordOfSalesViewModelTests
    {
        [Test]
        public void OnConstruction_AssertDefaults()
        {
            var subject = new ArtistRecordOfSalesViewModel(Mock.Of<IEmailService>(), TestData.Create<ArtistSales>());
            Assert.That(subject.SendInProgress, Is.False);
            Assert.That(subject.SendSuccessful, Is.False);
            Assert.That(subject.StatusMessage, Is.Null);
        }

        [Test]
        public void VerifySimpleSettersAndFormatting()
        {
            var artistSales = TestData.Create<ArtistSales>();
            var subject = new ArtistRecordOfSalesViewModel(Mock.Of<IEmailService>(), artistSales);
            subject.EmailAddress = "MyEmail";
            subject.CustomMessage = TestData.WellKnownString;

            Assert.That(subject.ToString(), Contains.Substring(subject.EmailAddress));
            Assert.That(subject.CustomMessage, Is.EqualTo(TestData.WellKnownString));
        }

        [Test]
        public void ToggleSentFlag_GivenFirstCall_ShouldSetToStatusToIgnored()
        {
            var subject = new ArtistRecordOfSalesViewModel(Mock.Of<IEmailService>(), TestData.Create<ArtistSales>());
            subject.ToggleSentFlag();

            Assert.That(subject.SendSuccessful, Is.True);
            Assert.That(subject.StatusMessage, Is.EqualTo("Ignored."));
        }

        [Test]
        public void ToggleSentFlag_GivenSecondCAll_ShouldSetToOriginalState()
        {
            var subject = new ArtistRecordOfSalesViewModel(Mock.Of<IEmailService>(), TestData.Create<ArtistSales>());
            subject.ToggleSentFlag();
            subject.ToggleSentFlag();

            Assert.That(subject.SendSuccessful, Is.False);
            Assert.That(subject.StatusMessage, Is.Null);
        }

        [Test]
        public async Task SendEmailAsync_GivenServiceThrowsException_SetsStatusToExceptionMessage()
        {
            var exception = new Exception(TestData.WellKnownString);
            var emailService = new Mock<IEmailService>();
            emailService.Setup(x => x.BuildRecordOfSalesEmail(It.IsAny<ArtistSales>())).Returns(new SendGridMessage());
            emailService.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>())).ThrowsAsync(exception);

            var subject = new ArtistRecordOfSalesViewModel(emailService.Object, TestData.Create<ArtistSales>());
            await subject.SendEmailAsync();

            Assert.That(subject.SendSuccessful, Is.False);
            Assert.That(subject.StatusMessage, Is.EqualTo(TestData.WellKnownString));
        }

        [Test]
        public async Task SendEmailAsync_GivenServiceSendsSuccessfully_SetsStatusToSuccess()
        {
            var emailService = new Mock<IEmailService>();
            emailService.Setup(x => x.BuildRecordOfSalesEmail(It.IsAny<ArtistSales>())).Returns(new SendGridMessage());
            emailService.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>())).ReturnsAsync(new EmailResponse(200));

            var artistSales = TestData.Create<ArtistSales>();
            var subject = new ArtistRecordOfSalesViewModel(emailService.Object, artistSales);
            subject.EmailAddress = TestData.WellKnownString;
            await subject.SendEmailAsync();

            Assert.That(subject.SendSuccessful, Is.True);
            Assert.That(subject.StatusMessage, Contains.Substring("Successful"));
            Assert.That(subject.StatusMessage, Contains.Substring(TestData.WellKnownString));
        }
    }
}
