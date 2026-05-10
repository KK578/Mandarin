using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Services.Transactions.External;
using Mandarin.Tests.Data;
using Mandarin.Transactions.External;
using Moq;
using NodaTime;
using Square;
using Square.Orders;
using Xunit;

namespace Mandarin.Services.Tests.Transactions.External
{
    public class SquareTransactionServiceTests
    {
        private static readonly LocalDate Start = new(2021, 06, 01);
        private static readonly LocalDate End = new(2021, 07, 01);

        private readonly Mock<ISquareClient> squareClient = new();
        private readonly Mock<ISquareTransactionMapper> transactionMapper = new();

        protected SquareTransactionServiceTests()
        {
            this.GivenSquareClientLocationApiReturnsData();
        }

        private ISquareTransactionService Subject => new SquareTransactionService(this.squareClient.Object);

        private void GivenSquareClientLocationApiReturnsData()
        {
            this.squareClient.Setup(x => x.Locations.ListAsync(It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ListLocationsResponse { Locations = new List<Location> { new() { Id = "Location1" } } });
        }

        private void GivenSquareClientOrdersApiReturnsData()
        {
            this.squareClient.Setup(x => x.Orders.SearchAsync(It.IsAny<SearchOrdersRequest>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => WellKnownTestData.DeserializeFromFile<SearchOrdersResponse>(WellKnownTestData.Square.OrdersApi.SearchOrders.SearchOrdersPage1));
            this.squareClient.Setup(x => x.Orders.SearchAsync(It.Is<SearchOrdersRequest>(y => y.Cursor == WellKnownTestData.Square.OrdersApi.SearchOrders.SearchOrdersPage2), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => WellKnownTestData.DeserializeFromFile<SearchOrdersResponse>(WellKnownTestData.Square.OrdersApi.SearchOrders.SearchOrdersPage2));
        }

        private Task<SearchOrdersRequest> GivenSquareClientOrdersApiCapturesRequest()
        {
            var tcs = new TaskCompletionSource<SearchOrdersRequest>();
            this.squareClient.Setup(x => x.Orders.SearchAsync(It.IsAny<SearchOrdersRequest>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
                .Callback((SearchOrdersRequest r, CancellationToken _) => tcs.SetResult(r))
                .ReturnsAsync(new SearchOrdersResponse());

            return tcs.Task;
        }

        private ManualResetEvent GivenSquareClientOrderApiWaitsToContinue()
        {
            var waitHandle = new ManualResetEvent(false);
            this.squareClient
                .Setup(x => x.Orders.SearchAsync(It.IsAny<SearchOrdersRequest>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
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
                .Returns(Observable.Return(MandarinFixture.Instance.NewTransaction with { ExternalTransactionId = ExternalTransactionId.Of("Order1") }));
            this.transactionMapper.Setup(x => x.MapToTransaction(It.Is<Order>(o => o.Id == "Order2")))
                .Returns(Observable.Return(MandarinFixture.Instance.NewTransaction with { ExternalTransactionId = ExternalTransactionId.Of("Order2") }));
        }

        public class GetAllOrdersTests : SquareTransactionServiceTests
        {
            [Fact]
            public async Task ShouldThrowExceptionWhenRequestIsCancelled()
            {
                var waitHandle = this.GivenSquareClientOrderApiWaitsToContinue();
                var cts = new CancellationTokenSource();
                var task = this.Subject.GetAllOrders(SquareTransactionServiceTests.Start, SquareTransactionServiceTests.End).ToList().ToTask(cts.Token);
                await task.WaitAsync(TimeSpan.FromMilliseconds(10), cts.Token);
                await cts.CancelAsync();
                waitHandle.Set();

                await task.Awaiting(x => x).Should().ThrowAsync<TaskCanceledException>();
            }

            [Fact]
            public async Task ShouldHaveCorrectSerializationOfDates()
            {
                var requestTask = this.GivenSquareClientOrdersApiCapturesRequest();
                await this.Subject.GetAllOrders(SquareTransactionServiceTests.Start, SquareTransactionServiceTests.End).ToList().ToTask();

                var request = await requestTask;
                request.Query.Filter.DateTimeFilter.CreatedAt.StartAt.Should().Be("2021-06-01T00:00:00Z");
                request.Query.Filter.DateTimeFilter.CreatedAt.EndAt.Should().Be("2021-07-01T00:00:00Z");
            }

            [Fact]
            public async Task ShouldContainAllTransactionsFromMultiplePages()
            {
                this.GivenSquareClientLocationApiReturnsData();
                this.GivenSquareClientOrdersApiReturnsData();
                this.GivenTransactionMapperMapsTo();
                var transactions = await this.Subject.GetAllOrders(SquareTransactionServiceTests.Start, SquareTransactionServiceTests.End).ToList();

                transactions.Should().HaveCount(2);
                transactions[0].Id.Should().Be("Order1");
                transactions[1].Id.Should().Be("Order2");
            }
        }
    }
}
