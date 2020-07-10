using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using AutoFixture;
using Bashi.Tests.Framework.Logging;
using Mandarin.Models.Artists;
using Mandarin.Services.Decorators;
using Mandarin.Services.Tests.Entity;
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
            Assert.ThrowsAsync<Exception>(() => subject.GetArtistsForDisplayAsync().ToList().ToTask());
            Assert.That(this.logger.LogEntries, Has.Count.EqualTo(1));
            Assert.That(this.logger.LogEntries[0].Exception, Is.EqualTo(exception));
        }

        [Test]
        public async Task GetArtistDetailsAsync_WhenServiceReturnsDataSuccessfully_ThenThereAreNoLogs()
        {
            this.GivenServiceReturnsData();
            var subject = new LoggingArtistServiceDecorator(this.service.Object, this.logger);
            await subject.GetArtistsForDisplayAsync().ToList();
            Assert.That(this.logger.LogEntries, Has.Count.Zero);
        }

        [Test]
        public void GetArtistDetailsForCommissionAsync_WhenServiceThrowsException_ThenItShouldBeLogged()
        {
            var exception = new Exception("Service failure.");
            this.GivenServiceThrowsException(exception);
            var subject = new LoggingArtistServiceDecorator(this.service.Object, this.logger);
            Assert.That(() => subject.GetArtistsForCommissionAsync(), Throws.Exception);
            Assert.That(this.logger.LogEntries, Has.Count.EqualTo(1));
            Assert.That(this.logger.LogEntries[0].Exception, Is.EqualTo(exception));
        }

        [Test]
        public async Task GetArtistDetailsForCommissionAsync_WhenServiceReturnsDataSuccessfully_ThenThereAreNoLogs()
        {
            this.GivenServiceReturnsData();
            var subject = new LoggingArtistServiceDecorator(this.service.Object, this.logger);
            await subject.GetArtistsForCommissionAsync();
            Assert.That(this.logger.LogEntries, Has.Count.Zero);
        }

        private void GivenServiceReturnsData()
        {
            var data = MandarinFixture.Instance.CreateMany<Stockist>().ToObservable();
            this.service.Setup(x => x.GetArtistsForDisplayAsync()).Returns(data).Verifiable();
            this.service.Setup(x => x.GetArtistsForCommissionAsync()).Returns(data).Verifiable();
        }

        private void GivenServiceThrowsException(Exception ex)
        {
            this.service.Setup(x => x.GetArtistsForDisplayAsync()).Throws(ex).Verifiable();
            this.service.Setup(x => x.GetArtistsForCommissionAsync()).Throws(ex).Verifiable();
        }
    }
}
