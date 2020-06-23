using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Bashi.Tests.Framework.Logging;
using Mandarin.Models.Artists;
using Mandarin.Services.Decorators;
using Moq;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Decorators
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
        public void GetArtistDetailsAsync_WhenServiceThrowsException_ThenItShouldBeLogged()
        {
            var exception = new Exception("Service failure.");
            this.GivenServiceThrowsException(exception);
            var subject = new LoggingArtistServiceDecorator(this.service.Object, this.logger);
            Assert.That(() => subject.GetArtistDetailsAsync(), Throws.Exception);
            Assert.That(this.logger.LogEntries, Has.Count.EqualTo(1));
            Assert.That(this.logger.LogEntries[0].Exception, Is.EqualTo(exception));
        }

        [Test]
        public async Task GetArtistDetailsAsync_WhenServiceReturnsDataSuccessfully_ThenThereAreNoLogs()
        {
            this.GivenServiceReturnsData();
            var subject = new LoggingArtistServiceDecorator(this.service.Object, this.logger);
            await subject.GetArtistDetailsAsync();
            Assert.That(this.logger.LogEntries, Has.Count.Zero);
        }

        [Test]
        public void GetArtistDetailsForCommissionAsync_WhenServiceThrowsException_ThenItShouldBeLogged()
        {
            var exception = new Exception("Service failure.");
            this.GivenServiceThrowsException(exception);
            var subject = new LoggingArtistServiceDecorator(this.service.Object, this.logger);
            Assert.That(() => subject.GetArtistDetailsForCommissionAsync(), Throws.Exception);
            Assert.That(this.logger.LogEntries, Has.Count.EqualTo(1));
            Assert.That(this.logger.LogEntries[0].Exception, Is.EqualTo(exception));
        }

        [Test]
        public async Task GetArtistDetailsForCommissionAsync_WhenServiceReturnsDataSuccessfully_ThenThereAreNoLogs()
        {
            this.GivenServiceReturnsData();
            var subject = new LoggingArtistServiceDecorator(this.service.Object, this.logger);
            await subject.GetArtistDetailsForCommissionAsync();
            Assert.That(this.logger.LogEntries, Has.Count.Zero);
        }

        private void GivenServiceReturnsData()
        {
            var data = TestData.Create<List<ArtistDetailsModel>>();
            this.service.Setup(x => x.GetArtistDetailsAsync()).ReturnsAsync(data).Verifiable();
            this.service.Setup(x => x.GetArtistDetailsForCommissionAsync()).ReturnsAsync(data).Verifiable();
        }

        private void GivenServiceThrowsException(Exception ex)
        {
            this.service.Setup(x => x.GetArtistDetailsAsync()).ThrowsAsync(ex).Verifiable();
            this.service.Setup(x => x.GetArtistDetailsForCommissionAsync()).ThrowsAsync(ex).Verifiable();
        }
    }
}
