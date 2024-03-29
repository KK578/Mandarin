﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Client.ViewModels.Commissions;
using Mandarin.Commissions;
using Mandarin.Emails;
using Mandarin.Tests.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Commissions
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
            this.recordOfSales = MandarinFixture.Instance.Create<RecordOfSales>();
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
                var subject = new ArtistRecordOfSalesViewModel(Mock.Of<IEmailService>(), null, this.recordOfSales);
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
                var exception = MandarinFixture.Instance.NewException;
                this.emailService.Setup(x => x.SendRecordOfSalesEmailAsync(It.IsAny<RecordOfSales>()))
                    .ThrowsAsync(exception);

                var subject = this.Subject;
                await subject.SendEmailAsync();

                subject.SendSuccessful.Should().BeFalse();
                subject.StatusMessage.Should().Be(exception.Message);
            }

            [Fact]
            public async Task ShouldShowServiceResponseOnSuccess()
            {
                var message = MandarinFixture.Instance.NewString;
                var response = new EmailResponse { IsSuccess = true, Message = $"Successfully sent to {message}!" };
                this.emailService.Setup(x => x.SendRecordOfSalesEmailAsync(It.IsAny<RecordOfSales>())).ReturnsAsync(response);

                var subject = this.Subject;
                subject.EmailAddress = message;
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
