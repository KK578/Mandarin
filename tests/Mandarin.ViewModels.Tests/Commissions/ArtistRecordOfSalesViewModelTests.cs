using System;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Mandarin.Models.Commissions;
using Mandarin.Services;
using Mandarin.Services.Objects;
using Mandarin.ViewModels.Commissions;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Commissions
{
    [TestFixture]
    public class ArtistRecordOfSalesViewModelTests
    {
        [Test]
        public void OnConstruction_AssertDefaults()
        {
            var subject = new ArtistRecordOfSalesViewModel(Mock.Of<IEmailService>(), null, null, TestData.Create<RecordOfSales>());
            Assert.That(subject.SendInProgress, Is.False);
            Assert.That(subject.SendSuccessful, Is.False);
            Assert.That(subject.StatusMessage, Is.Null);
        }

        [Test]
        public void VerifySimpleSettersAndFormatting()
        {
            var recordOfSales = TestData.Create<RecordOfSales>();
            var subject = new ArtistRecordOfSalesViewModel(Mock.Of<IEmailService>(), null, null, recordOfSales);
            subject.EmailAddress = "MyEmail";
            subject.CustomMessage = TestData.WellKnownString;

            Assert.That(subject.ToString(), Contains.Substring(subject.EmailAddress));
            Assert.That(subject.CustomMessage, Is.EqualTo(TestData.WellKnownString));
        }

        [Test]
        public void ToggleSentFlag_GivenFirstCall_ShouldSetToStatusToIgnored()
        {
            var subject = new ArtistRecordOfSalesViewModel(Mock.Of<IEmailService>(), null, null, TestData.Create<RecordOfSales>());
            subject.ToggleSentFlag();

            Assert.That(subject.SendSuccessful, Is.True);
            Assert.That(subject.StatusMessage, Is.EqualTo("Ignored."));
        }

        [Test]
        public void ToggleSentFlag_GivenSecondCAll_ShouldSetToOriginalState()
        {
            var subject = new ArtistRecordOfSalesViewModel(Mock.Of<IEmailService>(), null, null, TestData.Create<RecordOfSales>());
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
            emailService.Setup(x => x.SendRecordOfSalesEmailAsync(It.IsAny<RecordOfSales>())).ThrowsAsync(exception);

            var subject = new ArtistRecordOfSalesViewModel(emailService.Object, null, null, TestData.Create<RecordOfSales>());
            await subject.SendEmailAsync();

            Assert.That(subject.SendSuccessful, Is.False);
            Assert.That(subject.StatusMessage, Is.EqualTo(TestData.WellKnownString));
        }

        [Test]
        public async Task SendEmailAsync_GivenServiceSendsSuccessfully_SetsStatusToSuccess()
        {
            var emailService = new Mock<IEmailService>();
            var recordOfSales = TestData.Create<RecordOfSales>();
            var response = new EmailResponse { IsSuccess = true, Message = $"Successfully sent to {TestData.WellKnownString}!" };
            emailService.Setup(x => x.SendRecordOfSalesEmailAsync(It.IsAny<RecordOfSales>())).ReturnsAsync(response);

            var subject = new ArtistRecordOfSalesViewModel(emailService.Object, null, null, recordOfSales);
            subject.EmailAddress = TestData.WellKnownString;
            await subject.SendEmailAsync();

            Assert.That(subject.SendSuccessful, Is.True);
            Assert.That(subject.StatusMessage, Is.EqualTo(response.Message));
        }

        [Test]
        public void SetMessageFromTemplate_GivenFormat_TheMessageShouldBeUpdatedToFormat()
        {
            var data = new
            {
                Admin = new
                {
                    RecordOfSales = new
                    {
                        Templates = new
                        {
                            Sales = "For {0} there are sales. {1}",
                        },
                    },
                },
            };

            var name = TestData.Create<string>();
            var recordOfSales = TestData.Create<RecordOfSales>();
            var pageContentModel = new PageContentModel(JToken.FromObject(data));
            var httpContextAccessor = Mock.Of<IHttpContextAccessor>(x => x.HttpContext.User.Identity.Name == name);
            var subject = new ArtistRecordOfSalesViewModel(null, pageContentModel, httpContextAccessor, recordOfSales);

            subject.SetMessageFromTemplate(RecordOfSalesTemplateKey.Sales);

            Assert.That(subject.CustomMessage, Is.EqualTo($"For {recordOfSales.FirstName} there are sales. {name}"));
        }
    }
}
