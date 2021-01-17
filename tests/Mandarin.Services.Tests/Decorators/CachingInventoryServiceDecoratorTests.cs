using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using LazyCache;
using LazyCache.Providers;
using Mandarin.Commissions;
using Mandarin.Inventory;
using Mandarin.Services.Decorators;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Mandarin.Services.Tests.Decorators
{
    public class CachingInventoryServiceDecoratorTests
    {
        private readonly Mock<IInventoryService> inventoryService;
        private readonly IAppCache appCache;

        protected CachingInventoryServiceDecoratorTests()
        {
            this.inventoryService = new Mock<IInventoryService>();
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            this.appCache = new CachingService(new MemoryCacheProvider(memoryCache));
        }

        private IInventoryService Subject =>
            new CachingInventoryServiceDecorator(this.inventoryService.Object,
                                                 this.appCache,
                                                 NullLogger<CachingInventoryServiceDecorator>.Instance);

        private void GivenServiceReturnsData()
        {
            this.inventoryService.Setup(x => x.GetInventory())
                .Returns(TestData.Create<List<Product>>().ToObservable())
                .Verifiable();
            this.inventoryService.Setup(x => x.GetFixedCommissionAmounts())
                .Returns(TestData.Create<List<FixedCommissionAmount>>().ToObservable())
                .Verifiable();
        }

        private void GivenServiceThrowsException()
        {
            this.inventoryService.Setup(x => x.GetInventory())
                .Returns(() => Observable.Throw<Product>(new Exception("Service failure.")))
                .Verifiable();
            this.inventoryService.Setup(x => x.GetFixedCommissionAmounts())
                .Returns(() => Observable.Throw<FixedCommissionAmount>(new Exception("Service failure.")))
                .Verifiable();
        }

        private void GivenFixedCommissionAmountsExist()
        {
            this.appCache.Add("IInventoryService.GetFixedCommissionAmounts", TestData.Create<List<FixedCommissionAmount>>());
        }

        private async Task WhenServiceIsCalledMultipleTimes<T>(int times, Func<IObservable<T>> action)
        {
            for (var i = 0; i < times; i++)
            {
                await action().ToList().ToTask();
            }
        }

        public class FixedCommissionAmountTests : CachingInventoryServiceDecoratorTests
        {
            [Fact]
            public async Task ShouldClearCacheOnUnderlyingAddingSuccessfully()
            {
                this.GivenFixedCommissionAmountsExist();
                this.inventoryService.Setup(x => x.AddFixedCommissionAmount(It.IsAny<FixedCommissionAmount>()))
                    .Returns(Task.CompletedTask);
                await this.Subject.AddFixedCommissionAmount(TestData.Create<FixedCommissionAmount>());
                this.ThenFixedCommissionAmountCacheIsCleared();
            }

            [Fact]
            public async Task ShouldClearCacheOnUnderlyingUpdatingSuccessfully()
            {
                this.GivenFixedCommissionAmountsExist();
                this.inventoryService.Setup(x => x.UpdateFixedCommissionAmount(It.IsAny<FixedCommissionAmount>()))
                    .Returns(Task.CompletedTask);
                await this.Subject.UpdateFixedCommissionAmount(TestData.Create<FixedCommissionAmount>());
                this.ThenFixedCommissionAmountCacheIsCleared();
            }

            [Fact]
            public async Task ShouldClearCacheOnUnderlyingDeletingSuccessfully()
            {
                this.GivenFixedCommissionAmountsExist();
                this.inventoryService.Setup(x => x.DeleteFixedCommissionAmount(It.IsAny<string>()))
                    .Returns(Task.CompletedTask);
                await this.Subject.DeleteFixedCommissionAmount(It.IsAny<string>());
                this.ThenFixedCommissionAmountCacheIsCleared();
            }

            [Fact]
            public async Task ShouldAttemptToPopulateTheCacheIfAnExceptionOccurredPreviously()
            {
                this.GivenServiceThrowsException();
                await this.WhenServiceIsCalledMultipleTimes(5, () => this.Subject.GetFixedCommissionAmounts());
                this.inventoryService.Verify(x => x.GetFixedCommissionAmounts(), Times.Exactly(5));
            }

            [Fact]
            public async Task ShouldNotCallTheServiceIfCacheIsAlreadyPopulated()
            {
                this.GivenServiceReturnsData();
                await this.WhenServiceIsCalledMultipleTimes(5, () => this.Subject.GetFixedCommissionAmounts());
                this.inventoryService.Verify(x => x.GetFixedCommissionAmounts(), Times.Once());
            }

            private void ThenFixedCommissionAmountCacheIsCleared()
            {
                this.appCache.Get<List<FixedCommissionAmount>>("IInventoryService.GetFixedCommissionAmounts")
                    .Should()
                    .BeNull();
            }
        }

        public class ProductTests : CachingInventoryServiceDecoratorTests
        {
            [Fact]
            public async Task ShouldAttemptToPopulateTheCacheIfAnExceptionOccurredPreviously()
            {
                this.GivenServiceThrowsException();
                await this.WhenServiceIsCalledMultipleTimes(5, () => this.Subject.GetInventory());
                this.inventoryService.Verify(x => x.GetInventory(), Times.Exactly(5));
            }

            [Fact]
            public async Task ShouldNotCallTheServiceIfCacheIsAlreadyPopulated()
            {
                this.GivenServiceReturnsData();
                await this.WhenServiceIsCalledMultipleTimes(5, () => this.Subject.GetInventory());
                this.inventoryService.Verify(x => x.GetInventory(), Times.Once());
            }
        }
    }
}
