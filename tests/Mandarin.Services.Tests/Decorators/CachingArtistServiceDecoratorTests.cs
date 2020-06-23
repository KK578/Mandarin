using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using LazyCache;
using LazyCache.Providers;
using Mandarin.Models.Artists;
using Mandarin.Services.Decorators;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Decorators
{
    [TestFixture]
    public class CachingArtistServiceDecoratorTests
    {
        [Test]
        public async Task GetArtistDetailsAsync_GivenMultipleCalls_WhenServiceThrowsException_ThenServiceIsCalledEachTime()
        {
            var artistService = this.GivenServiceThrowsException();
            var appCache = this.GivenRealMemoryCache();
            var subject = new CachingArtistServiceDecorator(artistService.Object, appCache);
            await this.WhenServiceIsCalledMultipleTimes(5, () => subject.GetArtistDetailsAsync());
            artistService.Verify(x => x.GetArtistDetailsAsync(), Times.Exactly(5));
        }

        [Test]
        public async Task GetArtistDetailsAsync_GivenMultipleCalls_WhenServiceReturnsDataSuccessfullyFirstTime_ThenServiceIsOnlyCalledOnce()
        {
            var artistService = this.GivenServiceReturnsData();
            var appCache = this.GivenRealMemoryCache();
            var subject = new CachingArtistServiceDecorator(artistService.Object, appCache);
            await this.WhenServiceIsCalledMultipleTimes(5, () => subject.GetArtistDetailsAsync());
            artistService.Verify(x => x.GetArtistDetailsAsync(), Times.Once());
        }

        [Test]
        public async Task GetArtistDetailsForCommissionAsync_GivenMultipleCalls_WhenServiceThrowsException_ThenServiceIsCalledEachTime()
        {
            var artistService = this.GivenServiceThrowsException();
            var appCache = this.GivenRealMemoryCache();
            var subject = new CachingArtistServiceDecorator(artistService.Object, appCache);
            await this.WhenServiceIsCalledMultipleTimes(5, () => subject.GetArtistDetailsForCommissionAsync());
            artistService.Verify(x => x.GetArtistDetailsForCommissionAsync(), Times.Exactly(5));
        }

        [Test]
        public async Task GetArtistDetailsForCommissionAsync_GivenMultipleCalls_WhenServiceReturnsDataSuccessfullyFirstTime_ThenServiceIsOnlyCalledOnce()
        {
            var artistService = this.GivenServiceReturnsData();
            var appCache = this.GivenRealMemoryCache();
            var subject = new CachingArtistServiceDecorator(artistService.Object, appCache);
            await this.WhenServiceIsCalledMultipleTimes(5, () => subject.GetArtistDetailsForCommissionAsync());
            artistService.Verify(x => x.GetArtistDetailsForCommissionAsync(), Times.Once());
        }


        private Mock<IArtistService> GivenServiceReturnsData()
        {
            var artistService = new Mock<IArtistService>();
            var data = TestData.Create<List<ArtistDetailsModel>>();
            artistService.Setup(x => x.GetArtistDetailsAsync()).ReturnsAsync(data).Verifiable();
            artistService.Setup(x => x.GetArtistDetailsForCommissionAsync()).ReturnsAsync(data).Verifiable();

            return artistService;
        }

        private Mock<IArtistService> GivenServiceThrowsException()
        {
            var artistService = new Mock<IArtistService>();
            artistService.Setup(x => x.GetArtistDetailsAsync()).ThrowsAsync(new Exception("Service failure.")).Verifiable();
            artistService.Setup(x => x.GetArtistDetailsForCommissionAsync()).ThrowsAsync(new Exception("Service failure.")).Verifiable();

            return artistService;
        }

        private IAppCache GivenRealMemoryCache()
        {
            return new CachingService(new MemoryCacheProvider(new MemoryCache(Options.Create(new MemoryCacheOptions()))));
        }


        private async Task WhenServiceIsCalledMultipleTimes(int times, Func<Task> action)
        {
            for (var i = 0; i < times; i++)
            {
                await action();
            }
        }
    }
}
