using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Commissions;
using Mandarin.Emails;
using Mandarin.ViewModels.Commissions;
using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mandarin.ViewModels.Tests.Commissions
{
    public class ArtistRecordOfSalesViewModelTests
    {
        private const string CurrentUserName = "Fred";

        private readonly Mock<IEmailService> emailService;
        private readonly Mock<AuthenticationStateProvider> authenticationStateProvider;

        private readonly RecordOfSales recordOfSales;

        protected ArtistRecordOfSalesViewModelTests()
        {
            this.emailService = new Mock<IEmailService>();

            var claims = new[] { new Claim(ClaimTypes.Name, ArtistRecordOfSalesViewModelTests.CurrentUserName) };
            var identity = new ClaimsIdentity(claims, "MandarinTestIdentity");
            var principal = new ClaimsPrincipal(identity);
            this.authenticationStateProvider = new Mock<AuthenticationStateProvider>();
            this.authenticationStateProvider.Setup(x => x.GetAuthenticationStateAsync()).ReturnsAsync(new AuthenticationState(principal));
            this.recordOfSales = TestData.Create<RecordOfSales>();
        }

        private IArtistRecordOfSalesViewModel Subject =>
            new ArtistRecordOfSalesViewModel(this.emailService.Object,
                                             this.authenticationStateProvider.Object,
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
                var subject = new ArtistRecordOfSalesViewModel(Mock.Of<IEmailService>(), null, TestData.Create<RecordOfSales>());
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
            public async Task ShouldSetTheCustomMessageToFormattedTemplateString()
            {
                var subject = this.Subject;
                var template = new RecordOfSalesMessageTemplate
                {
                    Name = "Sales",
                    TemplateFormat = "For {0} there are sales. {1}",
                };

                await subject.SetMessageFromTemplateAsync(template);

                subject.CustomMessage.Should().Be($"For {this.recordOfSales.FirstName} there are sales. {ArtistRecordOfSalesViewModelTests.CurrentUserName}");
            }
        }
    }
}
