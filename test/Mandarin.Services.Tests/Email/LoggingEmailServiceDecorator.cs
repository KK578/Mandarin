using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Contact;
using Mandarin.Services.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using SendGrid.Helpers.Mail;

namespace Mandarin.Services.Tests.Email
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
        public void SendEmailAsync_WhenSuccessful_IsFallthrough()
        {
            var email = new SendGridMessage();
            email.SetFrom("SomeEmail@address.com");
            email.Subject = "Hello";
            email.PlainTextContent = "Content";

            var service = Mock.Of<IEmailService>(x => x.SendEmailAsync(It.IsAny<SendGridMessage>()) == Task.FromResult(new EmailResponse(200)));
            var logger = new TestableLogger<IEmailService>();
            var subject = new LoggingEmailServiceDecorator(service, logger);
            subject.SendEmailAsync(email);

            Assert.That(logger.LogEntries.Count, Is.EqualTo(1));
            Assert.That(logger.LogEntries[0].Message, Is.EqualTo("Sending Email: From=SomeEmail@address.com, Subject=Hello, Content=Content, Attachments=(null)"));

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

    public class TestableLogger<T> : TestableLogger, ILogger<T>
    {
    }

    public class TestableLogger : ILogger
    {
        private readonly List<TestLogEntry> logs = new List<TestLogEntry>();
        public IReadOnlyList<TestLogEntry> LogEntries => this.logs.AsReadOnly();

        public void Log<TState>(LogLevel logLevel,
                                EventId eventId,
                                TState state,
                                Exception exception,
                                Func<TState, Exception, string> formatter)
        {
            this.logs.Add(new TestLogEntry(logLevel, exception, formatter(state, exception)));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }

    public class TestLogEntry
    {
        public TestLogEntry(LogLevel logLevel, Exception exception, string message)
        {
            this.LogLevel = logLevel;
            this.Exception = exception;
            this.Message = message;
        }

        public LogLevel LogLevel { get; }
        public Exception Exception { get; }
        public string Message { get; }
    }
}
