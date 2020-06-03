using System;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Bashi.Tests.Framework.Logging;
using Mandarin.Models.Commissions;
using Mandarin.Models.Contact;
using Mandarin.Services.Decorators;
using Mandarin.Services.Objects;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services.Tests.Decorators
{
    [TestFixture]
    public class LoggingEmailServiceDecoratorTests
    {
        [Test]
        public async Task BuildEmailAsync_IsFallthrough()
        {
            var model = new ContactDetailsModel();
            var expected = new SendGridMessage();
            var service = Mock.Of<IEmailService>(x => x.BuildEmailAsync(model) == Task.FromResult(expected));
            var subject = new LoggingEmailServiceDecorator(service, NullLogger<IEmailService>.Instance);
            var result = await subject.BuildEmailAsync(model);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void BuildRecordOfSalesEmail_IsFallthrough()
        {
            var model = TestData.Create<SendRecordOfSalesModel>();
            var expected = new SendGridMessage();
            var service = Mock.Of<IEmailService>(x => x.BuildRecordOfSalesEmail(model) == expected);
            var subject = new LoggingEmailServiceDecorator(service, NullLogger<IEmailService>.Instance);
            var result = subject.BuildRecordOfSalesEmail(model);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void SendEmailAsync_WhenSuccessful_WritesLogCorrectly()
        {
            var email = new SendGridMessage();
            email.SetFrom("SomeEmail@address.com");
            email.Subject = "Hello";
            email.PlainTextContent = "Content";

            var service = Mock.Of<IEmailService>();
            var logger = new TestableLogger<IEmailService>();
            var subject = new LoggingEmailServiceDecorator(service, logger);
            subject.SendEmailAsync(email);

            Assert.That(logger.LogEntries.Count, Is.EqualTo(1));
            Assert.That(logger.LogEntries[0].Message, Contains.Substring("From=SomeEmail@address.com"));
            Assert.That(logger.LogEntries[0].Message, Contains.Substring("Subject=Hello"));
            Assert.That(logger.LogEntries[0].Message, Contains.Substring("Content=Content"));
            Assert.That(logger.LogEntries[0].Message, Contains.Substring("Attachments=0"));
        }

        [Test]
        public void SendEmailAsync_WhenSuccessfulWithAttachment_WritesLogCorrectly()
        {
            var email = new SendGridMessage();
            email.SetFrom("SomeEmail@address.com");
            email.AddAttachment("FileName", "Content");

            var service = Mock.Of<IEmailService>();
            var logger = new TestableLogger<IEmailService>();
            var subject = new LoggingEmailServiceDecorator(service, logger);
            subject.SendEmailAsync(email);

            Assert.That(logger.LogEntries.Count, Is.EqualTo(1));
            Assert.That(logger.LogEntries[0].Message, Contains.Substring("Attachments=1"));
        }

        [Test]
        public void SendEmailAsync_WhenUnderlyingThrows_ExceptionIsLogged()
        {
            var email = new SendGridMessage();
            email.SetFrom("SomeEmail@address.com");
            email.SetSubject("Hello");
            var ex = new Exception("Service didn't work.");
            var service = Mock.Of<IEmailService>(x => x.SendEmailAsync(It.IsAny<SendGridMessage>()) == Task.FromException<EmailResponse>(ex));
            var logger = new TestableLogger<IEmailService>();
            var subject = new LoggingEmailServiceDecorator(service, logger);
            subject.SendEmailAsync(email);

            Assert.That(logger.LogEntries.Count, Is.EqualTo(2));
            Assert.That(logger.LogEntries[1].Exception, Is.EqualTo(ex));
        }
    }
}
