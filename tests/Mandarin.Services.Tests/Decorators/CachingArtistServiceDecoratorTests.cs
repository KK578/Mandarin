using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using LazyCache;
using Mandarin.Models.Artists;
using Mandarin.Services.Decorators;
using Moq;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Decorators
{
    [TestFixture]
    public class CachingArtistServiceDecoratorTests
    {
        private Mock<IArtistService> service;
        private IAppCache appCache;

        [SetUp]
        public void SetUp()
        {
            this.service = new Mock<IArtistService>();
        }

        [Test]
        public async Task GetArtistDetailsAsync_GivenMultipleCalls_WhenServiceThrowsException_ThenServiceIsCalledEachTime()
        {
            this.GivenServiceThrowsException();
            this.GivenRealMemoryCache();
            await this.WhenServiceIsCalledMultipleTimes(5);
            this.service.Verify(x => x.GetArtistDetailsAsync(), Times.Exactly(5));
        }

        [Test]
        public async Task GetArtistsDetailsAsync_GivenMultipleCalls_WhenServiceReturnsDataSuccessfullyFirstTime_ThenServiceIsOnlyCalledOnce()
        {
            this.GivenServiceReturnsData();
            this.GivenRealMemoryCache();
            await this.WhenServiceIsCalledMultipleTimes(5);
            this.service.Verify(x => x.GetArtistDetailsAsync(), Times.Once());
        }

        private void GivenServiceReturnsData()
        {
            var data = TestData.Create<List<ArtistDetailsModel>>();
            this.service.Setup(x => x.GetArtistDetailsAsync())
                .ReturnsAsync(data)
                .Verifiable();
        }

        private void GivenRealMemoryCache()
        {
            this.appCache = new CachingService();
        }

        private void GivenServiceThrowsException()
        {
            this.service.Setup(x => x.GetArtistDetailsAsync())
                .ThrowsAsync(new Exception("Service failure."))
                .Verifiable();
        }

        private async Task WhenServiceIsCalledMultipleTimes(int times)
        {
            var subject = new CachingArtistServiceDecorator(this.service.Object, this.appCache);
            for (var i = 0; i < times; i++)
            {
                await subject.GetArtistDetailsAsync();
            }
        }
    }
}
