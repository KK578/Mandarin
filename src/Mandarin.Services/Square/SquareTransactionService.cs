using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Square;
using Square.Models;
using Transaction = Mandarin.Models.Transactions.Transaction;

namespace Mandarin.Services.Square
{
    internal sealed class SquareTransactionService : ITransactionService
    {
        private readonly ILogger<SquareTransactionService> logger;
        private readonly ISquareClient squareClient;
        private readonly ITransactionMapper transactionMapper;

        public SquareTransactionService(ILogger<SquareTransactionService> logger,
                                        ISquareClient squareClient,
                                        ITransactionMapper transactionMapper)
        {
            this.logger = logger;
            this.squareClient = squareClient;
            this.transactionMapper = transactionMapper;
        }

        public IObservable<Transaction> GetAllTransactions(DateTime start, DateTime end)
        {
            this.logger.LogInformation("Loading Square Transactions - Between {Start} and {End}", start, end);
            return Observable.Create<Order>(SubscribeToOrders)
                             .SelectMany(this.transactionMapper.MapToTransaction);

            async Task SubscribeToOrders(IObserver<Order> o, CancellationToken ct)
            {
                var builder = new SearchOrdersRequest.Builder();
                builder.LocationIds(await this.ListAllLocationsAsync(ct));
                builder.Limit(100);
                builder.Query(new SearchOrdersQuery.Builder()
                              .Filter(new SearchOrdersFilter.Builder()
                                      .StateFilter(new SearchOrdersStateFilter(new[] { "COMPLETED" }))
                                      .DateTimeFilter(new SearchOrdersDateTimeFilter.Builder()
                                                      .ClosedAt(new TimeRange.Builder()
                                                                .StartAt(start.ToString("s"))
                                                                .EndAt(end.ToString("s"))
                                                                .Build())
                                                      .Build())
                                      .Build())
                              .Sort(new SearchOrdersSort("CLOSED_AT"))
                              .Build());

                SearchOrdersResponse response = null;
                do
                {
                    var request = builder.Cursor(response?.Cursor).Build();
                    response = await this.squareClient.OrdersApi.SearchOrdersAsync(request, ct);
                    this.logger.LogInformation("Loading Square Transactions - Got {Count} Order(s).",
                                               response.Orders.Count);
                    foreach (var order in response.Orders)
                    {
                        o.OnNext(order);
                    }
                } while (response.Cursor != null);

                o.OnCompleted();
            }
        }

        // TODO: Move to shared location.
        private async Task<ReadOnlyCollection<string>> ListAllLocationsAsync(CancellationToken ct)
        {
            var locations = await this.squareClient.LocationsApi.ListLocationsAsync(ct);
            ct.ThrowIfCancellationRequested();
            return locations.Locations.Select(x => x.Id).ToList().AsReadOnly();
        }
    }
}
