using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using LazyCache;
using LazyCache.Mocks;
using LazyCache.Providers;
using Mandarin.Models.Inventory;
using Mandarin.Services.Decorators;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Decorators
{
    [TestFixture]
    public class CachingInventoryServiceDecoratorTests
    {
        private Mock<IInventoryService> service;
        private IAppCache appCache;

        [SetUp]
        public void SetUp()
        {
            this.service = new Mock<IInventoryService>();
        }

        [Test]
        public async Task GetInventory_GivenMultipleCalls_WhenServiceThrowsException_ThenServiceIsCalledEachTime()
        {
            this.GivenServiceThrowsException();
            this.GivenRealMemoryCache();
            await this.WhenServiceIsCalledMultipleTimes(5);
            this.service.Verify(x => x.GetInventory(), Times.Exactly(5));
        }

        [Test]
        public async Task GetInventory_GivenMultipleCalls_WhenServiceReturnsDataSuccessfullyFirstTime_ThenServiceIsOnlyCalledOnce()
        {
            this.GivenServiceReturnsData();
            this.GivenRealMemoryCache();
            await this.WhenServiceIsCalledMultipleTimes(5);
            this.service.Verify(x => x.GetInventory(), Times.Once());
        }

        private void GivenServiceReturnsData()
        {
            var data = TestData.Create<List<Product>>();
            this.service.Setup(x => x.GetInventory())
                .Returns(data.ToObservable())
                .Verifiable();
        }

        private void GivenRealMemoryCache()
        {
            this.appCache = new CachingService(new MemoryCacheProvider(new MemoryCache(Options.Create(new MemoryCacheOptions()))));
        }

        private void GivenServiceThrowsException()
        {
            this.service.Setup(x => x.GetInventory())
                .Throws(new Exception("Service failure."))
                .Verifiable();
        }

        private async Task WhenServiceIsCalledMultipleTimes(int times)
        {
            var subject = new CachingInventoryServiceDecorator(this.service.Object, this.appCache);
            for (var i = 0; i < times; i++)
            {
                await subject.GetInventory().ToList().ToTask();
            }
        }
    }
}
