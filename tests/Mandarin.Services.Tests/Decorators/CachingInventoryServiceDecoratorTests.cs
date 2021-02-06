using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using LazyCache;
using LazyCache.Providers;
using Mandarin.Inventory;
using Mandarin.Services.Inventory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Mandarin.Services.Tests.Decorators
{
    public class CachingInventoryServiceDecoratorTests
    {
        private readonly Mock<IProductService> inventoryService;
        private readonly IAppCache appCache;

        protected CachingInventoryServiceDecoratorTests()
        {
            this.inventoryService = new Mock<IProductService>();
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            this.appCache = new CachingService(new MemoryCacheProvider(memoryCache));
        }

        private IProductService Subject =>
            new CachingProductServiceDecorator(this.inventoryService.Object,
                                                 this.appCache,
                                                 NullLogger<CachingProductServiceDecorator>.Instance);

        private void GivenServiceReturnsData()
        {
            this.inventoryService.Setup(x => x.GetAllProductsAsync())
                .ReturnsAsync(TestData.Create<List<Product>>().AsReadOnly())
                .Verifiable();
        }

        private void GivenServiceThrowsException()
        {
            this.inventoryService.Setup(x => x.GetAllProductsAsync())
                .ThrowsAsync(new Exception("Service failure."))
                .Verifiable();
        }

        private Task WhenServiceIsCalledMultipleTimes<T>(int times, Func<IObservable<T>> action)
        {
            return this.WhenServiceIsCalledMultipleTimes(times, () => action().ToList().ToTask());
        }

        private async Task WhenServiceIsCalledMultipleTimes<T>(int times, Func<Task<T>> action)
        {
            for (var i = 0; i < times; i++)
            {
                await action();
            }
        }

        public class ProductTests : CachingInventoryServiceDecoratorTests
        {
            [Fact]
            public async Task ShouldAttemptToPopulateTheCacheIfAnExceptionOccurredPreviously()
            {
                this.GivenServiceThrowsException();
                await this.WhenServiceIsCalledMultipleTimes(5, () => this.Subject.GetAllProductsAsync());
                this.inventoryService.Verify(x => x.GetAllProductsAsync(), Times.Exactly(5));
            }

            [Fact]
            public async Task ShouldNotCallTheServiceIfCacheIsAlreadyPopulated()
            {
                this.GivenServiceReturnsData();
                await this.WhenServiceIsCalledMultipleTimes(5, () => this.Subject.GetAllProductsAsync());
                this.inventoryService.Verify(x => x.GetAllProductsAsync(), Times.Once());
            }
        }
    }
}
