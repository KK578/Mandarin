using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using LazyCache;
using LazyCache.Providers;
using Mandarin.Models.Commissions;
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
        [Test]
        public async Task GetFixedCommissionAmounts_GivenMultipleCalls_WhenServiceThrowsException_ThenServiceIsCalledEachTime()
        {
            var service = this.GivenServiceThrowsException();
            var appCache = this.GivenRealMemoryCache();
            var subject = new CachingInventoryServiceDecorator(service.Object, appCache);
            await this.WhenServiceIsCalledMultipleTimes(5, () => subject.GetFixedCommissionAmounts());
            service.Verify(x => x.GetFixedCommissionAmounts(), Times.Exactly(5));
        }

        [Test]
        public async Task GetFixedCommissionAmounts_GivenMultipleCalls_WhenServiceReturnsDataSuccessfullyFirstTime_ThenServiceIsOnlyCalledOnce()
        {
            var service = this.GivenServiceReturnsData();
            var appCache = this.GivenRealMemoryCache();
            var subject = new CachingInventoryServiceDecorator(service.Object, appCache);
            await this.WhenServiceIsCalledMultipleTimes(5, () => subject.GetFixedCommissionAmounts());
            service.Verify(x => x.GetFixedCommissionAmounts(), Times.Once());
        }

        [Test]
        public async Task GetInventory_GivenMultipleCalls_WhenServiceThrowsException_ThenServiceIsCalledEachTime()
        {
            var service = this.GivenServiceThrowsException();
            var appCache = this.GivenRealMemoryCache();
            var subject = new CachingInventoryServiceDecorator(service.Object, appCache);
            await this.WhenServiceIsCalledMultipleTimes(5, () => subject.GetInventory());
            service.Verify(x => x.GetInventory(), Times.Exactly(5));
        }

        [Test]
        public async Task GetInventory_GivenMultipleCalls_WhenServiceReturnsDataSuccessfullyFirstTime_ThenServiceIsOnlyCalledOnce()
        {
            var service = this.GivenServiceReturnsData();
            var appCache = this.GivenRealMemoryCache();
            var subject = new CachingInventoryServiceDecorator(service.Object, appCache);
            await this.WhenServiceIsCalledMultipleTimes(5, () => subject.GetInventory());
            service.Verify(x => x.GetInventory(), Times.Once());
        }

        private Mock<IInventoryService> GivenServiceReturnsData()
        {
            var service = new Mock<IInventoryService>();
            service.Setup(x => x.GetInventory()).Returns(TestData.Create<List<Product>>().ToObservable()).Verifiable();
            service.Setup(x => x.GetFixedCommissionAmounts()).Returns(TestData.Create<List<FixedCommissionAmount>>().ToObservable()).Verifiable();

            return service;
        }

        private Mock<IInventoryService> GivenServiceThrowsException()
        {
            var service = new Mock<IInventoryService>();
            service.Setup(x => x.GetInventory()).Throws(new Exception("Service failure.")).Verifiable();
            service.Setup(x => x.GetFixedCommissionAmounts()).Throws(new Exception("Service failure.")).Verifiable();

            return service;
        }

        private IAppCache GivenRealMemoryCache()
        {
            return new CachingService(new MemoryCacheProvider(new MemoryCache(Options.Create(new MemoryCacheOptions()))));
        }

        private async Task WhenServiceIsCalledMultipleTimes<T>(int times, Func<IObservable<T>> action)
        {
            for (var i = 0; i < times; i++)
            {
                await action().ToList().ToTask();
            }
        }
    }
}
