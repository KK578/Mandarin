using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Mandarin.Services.Square;
using Mandarin.Tests.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Square;
using Square.Models;
using Transaction = Mandarin.Models.Transactions.Transaction;

namespace Mandarin.Services.Tests.Square
{
    [TestFixture]
    public class SquareTransactionServiceTests
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        private Mock<ISquareClient> squareClient;
        private Mock<ITransactionMapper> transactionMapper;

        [TearDown]
        public void TearDown()
        {
            this.squareClient = null;
            this.transactionMapper = null;
        }

        [Test]
        public void GetTransaction_WhenRequestIsCancelled_ShouldThrowException()
        {
            this.GivenSquareClientLocationApiReturnsData();
            this.GivenTransactionMapperIsInitialized();
            var waitHandle = this.GivenSquareClientOrderApiWaitsToContinue();
            var cts = new CancellationTokenSource();
            var task = this.WhenListingTransactions(cts.Token);
            task.Wait(10);
            cts.Cancel();
            waitHandle.Set();
            Assert.ThrowsAsync<TaskCanceledException>(() => task);
        }

        [Test]
        public async Task GetTransaction_WhenServiceListsMultiplePages_ShouldContainAllObjects()
        {
            this.GivenSquareClientLocationApiReturnsData();
            this.GivenSquareClientOrdersApiReturnsData();
            this.GivenTransactionMapperMapsToFakeData();
            var transactions = await this.WhenListingTransactions();
            Assert.That(transactions.Count, Is.EqualTo(6));
            Assert.That(transactions[0].SquareId, Contains.Substring("squareId"));
        }

        private void GivenSquareClientLocationApiReturnsData()
        {
            this.squareClient ??= new Mock<ISquareClient>();
            this.squareClient.Setup(x => x.LocationsApi.ListLocationsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ListLocationsResponse(locations: new List<Location> { new Location("MyTestLocationId") }));
        }

        private void GivenSquareClientOrdersApiReturnsData()
        {
            this.squareClient ??= new Mock<ISquareClient>();
            this.squareClient.Setup(x => x.OrdersApi.SearchOrdersAsync(It.IsAny<SearchOrdersRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => SquareTransactionServiceTests.DeserializeFromFile<SearchOrdersResponse>(WellKnownTestData.Square.OrdersApi.SearchOrders.SearchOrdersPage1));
            this.squareClient.Setup(x => x.OrdersApi.SearchOrdersAsync(It.Is<SearchOrdersRequest>(x => x.Cursor == WellKnownTestData.Square.OrdersApi.SearchOrders.SearchOrdersPage2), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => SquareTransactionServiceTests.DeserializeFromFile<SearchOrdersResponse>(WellKnownTestData.Square.OrdersApi.SearchOrders.SearchOrdersPage2));
        }

        private ManualResetEvent GivenSquareClientOrderApiWaitsToContinue()
        {
            var waitHandle = new ManualResetEvent(false);
            this.squareClient ??= new Mock<ISquareClient>();
            this.squareClient
                .Setup(x => x.OrdersApi.SearchOrdersAsync(It.IsAny<SearchOrdersRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.Run(() =>
                {
                    waitHandle.WaitOne();
                    return new SearchOrdersResponse();
                }));

            return waitHandle;
        }

        private void GivenTransactionMapperIsInitialized()
        {
            this.transactionMapper ??= new Mock<ITransactionMapper>();
        }

        private void GivenTransactionMapperMapsToFakeData()
        {
            this.transactionMapper ??= new Mock<ITransactionMapper>();
            this.transactionMapper.Setup(x => x.MapToTransaction(It.IsAny<Order>()))
                .Returns(TestData.Create<List<Transaction>>().ToObservable());
        }

        private Task<IList<Transaction>> WhenListingTransactions(CancellationToken ct = default)
        {
            var subject = new SquareTransactionService(NullLogger<SquareTransactionService>.Instance, this.squareClient.Object, this.transactionMapper.Object);
            return subject.GetAllTransactions(DateTime.Now, DateTime.Now).ToList().ToTask(ct);
        }

        private static T DeserializeFromFile<T>(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new StreamReader(fs);
            using var jsonReader = new JsonTextReader(reader);
            return SquareTransactionServiceTests.Serializer.Deserialize<T>(jsonReader);
        }
    }
}
