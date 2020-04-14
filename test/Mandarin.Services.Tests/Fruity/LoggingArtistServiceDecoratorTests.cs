using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Bashi.Tests.Framework.Logging;
using Mandarin.Models.Artists;
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

        [Test]
        public async Task GetArtistsDetailsAsync_GivenMultipleCalls_WhenServiceReturnsDataSuccessfullyFirstTime_ThenServiceIsOnlyCalledOnce()
        {
            this.GivenServiceReturnsData();
            await this.WhenServiceIsCalled();
            this.service.Verify(x => x.GetArtistDetailsAsync(), Times.Once());
        }

        private void GivenServiceThrowsException(Exception ex)
        {
            this.service.Setup(x => x.GetArtistDetailsAsync())
                .ThrowsAsync(ex)
                .Verifiable();
        }

        private void GivenServiceReturnsData()
        {
            var data = TestData.Create<List<ArtistDetailsModel>>();
            this.service.Setup(x => x.GetArtistDetailsAsync())
                .ReturnsAsync(data)
                .Verifiable();
        }

        private Task WhenServiceIsCalled()
        {
            var subject = new LoggingArtistServiceDecorator(this.service.Object, this.logger);
            return subject.GetArtistDetailsAsync();
        }
    }
}
