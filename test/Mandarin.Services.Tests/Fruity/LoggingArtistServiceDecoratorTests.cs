using System;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Logging;
using Mandarin.Services.Fruity;
using Moq;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Fruity
{
    [TestFixture]
    public class LoggingArtistServiceDecoratorTests
    {
        private Mock<IArtistService> service;
        private TestableLogger<IArtistService> logger;

        [SetUp]
        public void SetUp()
        {
            this.service = new Mock<IArtistService>();
            this.logger = new TestableLogger<IArtistService>();
        }

        [Test]
        public void GetArtistDetailsAsync_GivenMultipleCalls_WhenServiceThrowsException_ThenServiceIsCalledEachTime()
        {
            var exception = new Exception("Service failure.");
            this.GivenServiceThrowsException(exception);
            Assert.That(this.WhenServiceIsCalled, Throws.Exception);
            Assert.That(this.logger.LogEntries, Has.Count.EqualTo(1));
            Assert.That(this.logger.LogEntries[0].Exception, Is.EqualTo(exception));
        }

        private void GivenServiceThrowsException(Exception ex)
        {
            this.service.Setup(x => x.GetArtistDetailsAsync())
                .ThrowsAsync(ex)
                .Verifiable();
        }

        private Task WhenServiceIsCalled()
        {
            var subject = new LoggingArtistServiceDecorator(this.service.Object, this.logger);
            return subject.GetArtistDetailsAsync();
        }
    }
}
