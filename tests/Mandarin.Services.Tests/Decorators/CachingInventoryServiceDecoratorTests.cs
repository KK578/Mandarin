using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Mandarin.Services.Decorators;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using Square.Models;

namespace Mandarin.Services.Tests.Decorators
{
    [TestFixture]
    public class CachingInventoryServiceDecoratorTests
    {
        private Mock<IInventoryService> service;
        private IMemoryCache memoryCache;

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
            var data = new List<CatalogObject>()
            {
                new CatalogObject("Type", "Id")
            };
            this.service.Setup(x => x.GetInventory())
                .Returns(data.ToObservable())
                .Verifiable();
        }

        private void GivenRealMemoryCache()
        {
            this.memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        private void GivenServiceThrowsException()
        {
            this.service.Setup(x => x.GetInventory())
                .Throws(new Exception("Service failure."))
                .Verifiable();
        }

        private async Task WhenServiceIsCalledMultipleTimes(int times)
        {
            var subject = new CachingInventoryServiceDecorator(this.service.Object, this.memoryCache);
            for (var i = 0; i < times; i++)
            {
                await subject.GetInventory().ToList().ToTask();
            }
        }
    }
}
