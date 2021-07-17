﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Services.Transactions;
using Mandarin.Tests.Data;
using Mandarin.Tests.Data.Extensions;
using Mandarin.Transactions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Square;
using Square.Models;
using Xunit;
using Transaction = Mandarin.Transactions.Transaction;

namespace Mandarin.Services.Tests.Transactions
{
    public class SquareTransactionServiceTests
    {
        private readonly Mock<ISquareClient> squareClient = new();
        private readonly Mock<ITransactionMapper> transactionMapper = new();

        protected SquareTransactionServiceTests()
        {
            this.GivenSquareClientLocationApiReturnsData();
        }

        private ITransactionService Subject =>
            new SquareTransactionService(NullLogger<SquareTransactionService>.Instance,
                                         this.squareClient.Object,
                                         this.transactionMapper.Object);

        private void GivenSquareClientLocationApiReturnsData()
        {
            this.squareClient.Setup(x => x.LocationsApi.ListLocationsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ListLocationsResponse(locations: new List<Location> { new("Location1") }));
        }

        private void GivenSquareClientOrdersApiReturnsData()
        {
            this.squareClient.Setup(x => x.OrdersApi.SearchOrdersAsync(It.IsAny<SearchOrdersRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => WellKnownTestData.DeserializeFromFile<SearchOrdersResponse>(WellKnownTestData.Square.OrdersApi.SearchOrders.SearchOrdersPage1));
            this.squareClient.Setup(x => x.OrdersApi.SearchOrdersAsync(It.Is<SearchOrdersRequest>(y => y.Cursor == WellKnownTestData.Square.OrdersApi.SearchOrders.SearchOrdersPage2), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => WellKnownTestData.DeserializeFromFile<SearchOrdersResponse>(WellKnownTestData.Square.OrdersApi.SearchOrders.SearchOrdersPage2));
        }

        private ManualResetEvent GivenSquareClientOrderApiWaitsToContinue()
        {
            var waitHandle = new ManualResetEvent(false);
            this.squareClient
                .Setup(x => x.OrdersApi.SearchOrdersAsync(It.IsAny<SearchOrdersRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.Run(() =>
                {
                    waitHandle.WaitOne();
                    return new SearchOrdersResponse();
                }));

            return waitHandle;
        }

        private void GivenTransactionMapperMapsTo()
        {
            this.transactionMapper.Setup(x => x.MapToTransaction(It.Is<Order>(o => o.Id == "Order1")))
                .Returns(Observable.Return(TestData.Create<Transaction>().WithTransactionId(new TransactionId("Order1"))));
            this.transactionMapper.Setup(x => x.MapToTransaction(It.Is<Order>(o => o.Id == "Order2")))
                .Returns(Observable.Return(TestData.Create<Transaction>().WithTransactionId(new TransactionId("Order2"))));
        }

        public class GetAllTransactionsTests : SquareTransactionServiceTests
        {
            [Fact]
            public void GetTransaction_WhenRequestIsCancelled_ShouldThrowException()
            {
                var waitHandle = this.GivenSquareClientOrderApiWaitsToContinue();
                var cts = new CancellationTokenSource();
                var task = this.Subject.GetAllTransactions(DateTime.Now, DateTime.Now).ToList().ToTask(cts.Token);
                task.Wait(10);
                cts.Cancel();
                waitHandle.Set();

                task.Awaiting(x => x).Should().ThrowAsync<TaskCanceledException>();
            }

            [Fact]
            public async Task GetTransaction_WhenServiceListsMultiplePages_ShouldContainAllObjects()
            {
                this.GivenSquareClientLocationApiReturnsData();
                this.GivenSquareClientOrdersApiReturnsData();
                this.GivenTransactionMapperMapsTo();
                var transactions = await this.Subject.GetAllTransactions(DateTime.Now, DateTime.Now).ToList();

                transactions.Should().HaveCount(2);
                transactions[0].SquareId.Should().Be(new TransactionId("Order1"));
                transactions[1].SquareId.Should().Be(new TransactionId("Order2"));
            }
        }
    }
}
