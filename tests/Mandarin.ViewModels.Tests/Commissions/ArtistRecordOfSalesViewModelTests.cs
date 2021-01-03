using System;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Commissions;
using Mandarin.Emails;
using Mandarin.ViewModels.Commissions;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mandarin.ViewModels.Tests.Commissions
{
    public class ArtistRecordOfSalesViewModelTests
    {
        private const string CurrentUserName = "Fred";

        private readonly Mock<IEmailService> emailService;
        private readonly PageContentModel pageContentModel;
        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly RecordOfSales recordOfSales;

        protected ArtistRecordOfSalesViewModelTests()
        {
            var data = new { Admin = new { RecordOfSales = new { Templates = new { Sales = "For {0} there are sales. {1}" } } } };

            this.emailService = new Mock<IEmailService>();
            this.pageContentModel = new PageContentModel(JToken.FromObject(data));
            this.httpContextAccessor = Mock.Of<IHttpContextAccessor>(x => x.HttpContext.User.Identity.Name == ArtistRecordOfSalesViewModelTests.CurrentUserName);
            this.recordOfSales = TestData.Create<RecordOfSales>();
        }

        private IArtistRecordOfSalesViewModel Subject =>
            new ArtistRecordOfSalesViewModel(this.emailService.Object,
                                             this.pageContentModel,
                                             this.httpContextAccessor,
                                             this.recordOfSales);

        public class GeneralTests : ArtistRecordOfSalesViewModelTests
        {
            [Fact]
            public void OnConstruction_AssertDefaults()
            {
                var subject = this.Subject;
                subject.SendInProgress.Should().BeFalse();
                subject.SendSuccessful.Should().BeFalse();
                subject.StatusMessage.Should().BeNull();
            }

            [Fact]
            public void VerifySimpleSettersAndFormatting()
            {
                var subject = this.Subject;
                subject.EmailAddress = "MyEmail";
                subject.CustomMessage = TestData.WellKnownString;

                subject.ToString().Should().Contain(subject.EmailAddress);
                subject.CustomMessage.Should().Be(TestData.WellKnownString);
            }
        }

        public class ToggleSentFlagTests : ArtistRecordOfSalesViewModelTests
        {
            [Fact]
            public void ShouldSetStatusToIgnored()
            {
                var subject = this.Subject;
                subject.ToggleSentFlag();

                subject.SendSuccessful.Should().BeTrue();
                subject.StatusMessage.Should().Be("Ignored.");
            }

            [Fact]
            public void ShouldReturnStatusToNullIfPreviouslyIgnored()
            {
                var subject = new ArtistRecordOfSalesViewModel(Mock.Of<IEmailService>(), null, null, TestData.Create<RecordOfSales>());
                subject.ToggleSentFlag();
                subject.ToggleSentFlag();

                subject.SendSuccessful.Should().BeFalse();
                subject.StatusMessage.Should().BeNull();
            }
        }

        public class SendEmailAsyncTests : ArtistRecordOfSalesViewModelTests
        {
            [Fact]
            public async Task ShouldDisplayErrorIfServiceThrows()
            {
                var exception = new Exception(TestData.WellKnownString);
                this.emailService.Setup(x => x.SendRecordOfSalesEmailAsync(It.IsAny<RecordOfSales>()))
                    .ThrowsAsync(exception);

                var subject = this.Subject;
                await subject.SendEmailAsync();

                subject.SendSuccessful.Should().BeFalse();
                subject.StatusMessage.Should().Be(TestData.WellKnownString);
            }

            [Fact]
            public async Task ShouldShowServiceResponseOnSuccess()
            {
                var response = new EmailResponse { IsSuccess = true, Message = $"Successfully sent to {TestData.WellKnownString}!" };
                this.emailService.Setup(x => x.SendRecordOfSalesEmailAsync(It.IsAny<RecordOfSales>())).ReturnsAsync(response);

                var subject = this.Subject;
                subject.EmailAddress = TestData.WellKnownString;
                await subject.SendEmailAsync();

                subject.SendSuccessful.Should().BeTrue();
                subject.StatusMessage.Should().Be(response.Message);
            }
        }

        public class SetMessageFromTemplateTests : ArtistRecordOfSalesViewModelTests
        {
            [Fact]
            public void ShouldSetTheCustomMessageToFormattedTemplateString()
            {
                var subject = this.Subject;
                subject.SetMessageFromTemplate(RecordOfSalesTemplateKey.Sales);

                subject.CustomMessage.Should().Be($"For {this.recordOfSales.FirstName} there are sales. {ArtistRecordOfSalesViewModelTests.CurrentUserName}");
            }
        }
    }
}
