using System;
using Bashi.Tests.Framework.Data;
using Bashi.Tests.Framework.Logging;
using Mandarin.Models.Commissions;
using Mandarin.Services.Decorators;
using Mandarin.Services.Objects;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Decorators
{
    [TestFixture]
    public class LoggingEmailServiceDecoratorTests
    {
        private RecordOfSales recordOfSales;
        private Mock<IEmailService> underlyingService;
        private TestableLogger<LoggingEmailServiceDecorator> logger;

        private IEmailService Subject => new LoggingEmailServiceDecorator(this.underlyingService.Object, this.logger);

        [SetUp]
        public void SetUp()
        {
            this.recordOfSales = TestData.Create<RecordOfSales>();
            this.underlyingService = new Mock<IEmailService>();
            this.logger = new TestableLogger<LoggingEmailServiceDecorator>();
        }

        private void GivenUnderlyingRespondsWith(int statusCode)
        {
            var response = new EmailResponse(statusCode);
            this.underlyingService.Setup(x => x.SendRecordOfSalesEmailAsync(this.recordOfSales))
                .ReturnsAsync(response);
        }

        private void GivenUnderlyingThrows(Exception ex)
        {
            this.underlyingService.Setup(x => x.SendRecordOfSalesEmailAsync(this.recordOfSales))
                .ThrowsAsync(ex);
        }

        [TestFixture]
        private class SendRecordOfSalesEmailAsyncTests : LoggingEmailServiceDecoratorTests
        {
            [Test]
            public void ShouldWriteWarningLogEntryWhenResponseCodeUnsuccessful()
            {
                this.GivenUnderlyingRespondsWith(500);
                this.Subject.SendRecordOfSalesEmailAsync(this.recordOfSales);

                Assert.That(this.logger.LogEntries.Count, Is.EqualTo(1));
                Assert.That(this.logger.LogEntries[0].LogLevel, Is.EqualTo(LogLevel.Warning));
                Assert.That(this.logger.LogEntries[0].Message, Contains.Substring("Email sent with errors"));
            }

            [Test]
            public void ShouldWriteExceptionLogEntryWhenUnderlyingServiceThrows()
            {
                var ex = new Exception("Service didn't work.");
                this.GivenUnderlyingThrows(ex);
                this.Subject.SendRecordOfSalesEmailAsync(this.recordOfSales);

                Assert.That(this.logger.LogEntries.Count, Is.EqualTo(1));
                Assert.That(this.logger.LogEntries[0].LogLevel, Is.EqualTo(LogLevel.Error));
                Assert.That(this.logger.LogEntries[0].Exception, Is.EqualTo(ex));
            }

            [Test]
            public void ShouldWriteSuccessfulLogWhenUnderlyingIsSuccessful()
            {
                this.GivenUnderlyingRespondsWith(200);
                this.Subject.SendRecordOfSalesEmailAsync(this.recordOfSales);

                Assert.That(this.logger.LogEntries.Count, Is.EqualTo(1));
                Assert.That(this.logger.LogEntries[0].LogLevel, Is.EqualTo(LogLevel.Information));
                Assert.That(this.logger.LogEntries[0].Message, Contains.Substring("Sent email successfully"));
            }
        }
    }
}
