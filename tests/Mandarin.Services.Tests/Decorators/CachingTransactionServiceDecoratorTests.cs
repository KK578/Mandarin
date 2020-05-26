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
    public class CachingTransactionServiceDecoratorTests
    {
        private static readonly DateTime StartDate = new DateTime(2020, 05, 26);
        private static readonly DateTime EndDate = new DateTime(2020, 05, 27);

        private Mock<ITransactionService> service;
        private IMemoryCache memoryCache;

        [SetUp]
        public void SetUp()
        {
            this.service = new Mock<ITransactionService>();
        }

        [Test]
        public async Task GetAllTransactions_GivenMultipleCalls_WhenServiceThrowsException_ThenServiceIsCalledEachTime()
        {
            this.GivenServiceThrowsException();
            this.GivenRealMemoryCache();
            await this.WhenServiceIsCalledMultipleTimes(5);
            this.service.Verify(x => x.GetAllTransactions(CachingTransactionServiceDecoratorTests.StartDate, CachingTransactionServiceDecoratorTests.EndDate), Times.Exactly(5));
        }

        [Test]
        public async Task GetAllTransactions_GivenMultipleCalls_WhenServiceReturnsDataSuccessfullyFirstTime_ThenServiceIsOnlyCalledOnce()
        {
            this.GivenServiceReturnsData();
            this.GivenRealMemoryCache();
            await this.WhenServiceIsCalledMultipleTimes(5);
            this.service.Verify(x => x.GetAllTransactions(CachingTransactionServiceDecoratorTests.StartDate, CachingTransactionServiceDecoratorTests.EndDate), Times.Once());
        }

        [Test]
        public async Task GetAllTransactions_GivenCallsToDifferentDates_ServiceIsCalledPerDateCombination()
        {
            this.GivenServiceReturnsData();
            this.GivenRealMemoryCache();

            var subject = new CachingTransactionServiceDecorator(this.service.Object, this.memoryCache);
            var startDate = CachingTransactionServiceDecoratorTests.StartDate;
            var midDate = CachingTransactionServiceDecoratorTests.StartDate.AddDays(1);
            var endDate = CachingTransactionServiceDecoratorTests.EndDate;
            await subject.GetAllTransactions(startDate, endDate).ToList().ToTask();
            await subject.GetAllTransactions(startDate, midDate).ToList().ToTask();
            await subject.GetAllTransactions(startDate, midDate).ToList().ToTask();
            await subject.GetAllTransactions(midDate, endDate).ToList().ToTask();

            this.service.Verify(x => x.GetAllTransactions(startDate, endDate), Times.Once());
            this.service.Verify(x => x.GetAllTransactions(startDate, midDate), Times.Once());
            this.service.Verify(x => x.GetAllTransactions(midDate, endDate), Times.Once());
        }

        private void GivenServiceReturnsData()
        {
            var data = new List<Order>()
            {
                new Order("LocationId", "Id")
            };
            this.service.Setup(x => x.GetAllTransactions(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(data.ToObservable())
                .Verifiable();
        }

        private void GivenRealMemoryCache()
        {
            this.memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        private void GivenServiceThrowsException()
        {
            this.service.Setup(x => x.GetAllTransactions(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Throws(new Exception("Service failure."))
                .Verifiable();
        }

        private async Task WhenServiceIsCalledMultipleTimes(int times)
        {
            var subject = new CachingTransactionServiceDecorator(this.service.Object, this.memoryCache);
            for (var i = 0; i < times; i++)
            {
                await subject.GetAllTransactions(CachingTransactionServiceDecoratorTests.StartDate, CachingTransactionServiceDecoratorTests.EndDate).ToList().ToTask();
            }
        }
    }
}
