using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using LazyCache;
using LazyCache.Providers;
using Mandarin.Services.Common;
using Mandarin.Transactions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Mandarin.Services.Tests.Decorators
{
    public class CachingTransactionServiceDecoratorTests
    {
        private static readonly DateTime StartDate = new(2020, 05, 26);
        private static readonly DateTime EndDate = new(2020, 05, 27);

        private readonly Mock<ITransactionService> transactionService;
        private readonly IAppCache appCache;

        protected CachingTransactionServiceDecoratorTests()
        {
            this.transactionService = new Mock<ITransactionService>();
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            this.appCache = new CachingService(new MemoryCacheProvider(memoryCache));
        }

        private ITransactionService Subject =>
            new CachingTransactionServiceDecorator(this.transactionService.Object,
                                                   this.appCache,
                                                   NullLogger<CachingTransactionServiceDecorator>.Instance);

        private void GivenServiceReturnsData()
        {
            var data = TestData.Create<List<Transaction>>();
            this.transactionService.Setup(x => x.GetAllTransactions(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(data.ToObservable())
                .Verifiable();
        }

        private void GivenServiceThrowsException()
        {
            this.transactionService.Setup(x => x.GetAllTransactions(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Observable.Throw<Transaction>(new Exception("Service failure.")))
                .Verifiable();
        }

        public class TransactionTests : CachingTransactionServiceDecoratorTests
        {
            [Fact]
            public async Task ShouldAttemptToPopulateTheCacheIfAnExceptionOccurredPreviously()
            {
                this.GivenServiceThrowsException();
                await this.WhenServiceIsCalledMultipleTimes(5);
                this.transactionService.Verify(x => x.GetAllTransactions(CachingTransactionServiceDecoratorTests.StartDate, CachingTransactionServiceDecoratorTests.EndDate), Times.Exactly(5));
            }

            [Fact]
            public async Task ShouldNotCallTheServiceIfCacheIsAlreadyPopulated()
            {
                this.GivenServiceReturnsData();
                await this.WhenServiceIsCalledMultipleTimes(5);
                this.transactionService.Verify(x => x.GetAllTransactions(CachingTransactionServiceDecoratorTests.StartDate, CachingTransactionServiceDecoratorTests.EndDate), Times.Once());
            }

            [Fact]
            public async Task ShouldCallServiceOncePerDateCombination()
            {
                this.GivenServiceReturnsData();

                var startDate = CachingTransactionServiceDecoratorTests.StartDate;
                var midDate = CachingTransactionServiceDecoratorTests.StartDate.AddDays(1);
                var endDate = CachingTransactionServiceDecoratorTests.EndDate;

                var subject = this.Subject;
                await subject.GetAllTransactions(startDate, endDate).ToList().ToTask();
                await subject.GetAllTransactions(startDate, midDate).ToList().ToTask();
                await subject.GetAllTransactions(startDate, midDate).ToList().ToTask();
                await subject.GetAllTransactions(midDate, endDate).ToList().ToTask();

                this.transactionService.Verify(x => x.GetAllTransactions(startDate, endDate), Times.Once());
                this.transactionService.Verify(x => x.GetAllTransactions(startDate, midDate), Times.Once());
                this.transactionService.Verify(x => x.GetAllTransactions(midDate, endDate), Times.Once());
            }

            private async Task WhenServiceIsCalledMultipleTimes(int times)
            {
                for (var i = 0; i < times; i++)
                {
                    await this.Subject.GetAllTransactions(CachingTransactionServiceDecoratorTests.StartDate, CachingTransactionServiceDecoratorTests.EndDate).ToList();
                }
            }
        }
    }
}
