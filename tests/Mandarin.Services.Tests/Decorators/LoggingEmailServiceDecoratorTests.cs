using System;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Bashi.Tests.Framework.Logging;
using Mandarin.Models.Commissions;
using Mandarin.Services.Decorators;
using Mandarin.Services.Objects;
using Moq;
using NUnit.Framework;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services.Tests.Decorators
{
    [TestFixture]
    public class LoggingEmailServiceDecoratorTests
    {
        [Test]
        public void BuildRecordOfSalesEmail_IsFallthrough()
        {
            var model = TestData.Create<RecordOfSales>();
            var logger = new TestableLogger<IEmailService>();
            var email = new SendGridMessage();
            email.SetFrom("SomeEmail@address.com");
            email.SetSubject("Hello");
            email.AddAttachment("FileName", "Content");
            var service = Mock.Of<IEmailService>(x => x.BuildRecordOfSalesEmail(model) == email);
            var subject = new LoggingEmailServiceDecorator(service, logger);
            var result = subject.BuildRecordOfSalesEmail(model);

            Assert.That(result, Is.EqualTo(email));
            Assert.That(logger.LogEntries.Count, Is.EqualTo(1));
        }

        [Test]
        public void SendEmailAsync_WhenResponseCodeUnsuccessful_WritesLogCorrectly()
        {
            var email = new SendGridMessage();
            var response = new EmailResponse(500);
            var service = Mock.Of<IEmailService>(x => x.SendEmailAsync(email) == Task.FromResult(response));
            var logger = new TestableLogger<IEmailService>();

            var subject = new LoggingEmailServiceDecorator(service, logger);
            subject.SendEmailAsync(email);

            Assert.That(logger.LogEntries.Count, Is.EqualTo(1));
            Assert.That(logger.LogEntries[0].Message, Contains.Substring("Email sent with errors"));
        }

        [Test]
        public void SendEmailAsync_WhenUnderlyingThrows_ExceptionIsLogged()
        {
            var email = new SendGridMessage();
            var ex = new Exception("Service didn't work.");
            var service = Mock.Of<IEmailService>(x => x.SendEmailAsync(It.IsAny<SendGridMessage>()) == Task.FromException<EmailResponse>(ex));
            var logger = new TestableLogger<IEmailService>();

            var subject = new LoggingEmailServiceDecorator(service, logger);
            subject.SendEmailAsync(email);

            Assert.That(logger.LogEntries.Count, Is.EqualTo(1));
            Assert.That(logger.LogEntries[0].Exception, Is.EqualTo(ex));
        }

        [Test]
        public void SendEmailAsync_WhenSuccessful_WritesLogCorrectly()
        {
            var email = new SendGridMessage();
            var response = new EmailResponse(200);
            var service = Mock.Of<IEmailService>(x => x.SendEmailAsync(email) == Task.FromResult(response));
            var logger = new TestableLogger<IEmailService>();
            var subject = new LoggingEmailServiceDecorator(service, logger);
            subject.SendEmailAsync(email);

            Assert.That(logger.LogEntries.Count, Is.EqualTo(1));
            Assert.That(logger.LogEntries[0].Message, Contains.Substring("Sent email successfully"));
        }
    }
}
