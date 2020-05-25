using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Square;
using Square.Models;

namespace Mandarin.Services.Square
{
    internal sealed class SquareTransactionService : ITransactionService
    {
        private readonly ILogger<SquareTransactionService> logger;
        private readonly ISquareClient squareClient;

        public SquareTransactionService(ILogger<SquareTransactionService> logger, ISquareClient squareClient)
        {
            this.logger = logger;
            this.squareClient = squareClient;
        }

        public IObservable<Order> GetAllTransactions(DateTime start, DateTime end)
        {
            return Observable.Create<Order>(SubscribeToOrders);

            async Task SubscribeToOrders(IObserver<Order> o, CancellationToken ct)
            {
                var builder = new SearchOrdersRequest.Builder()
                              .LocationIds(await this.ListAllLocationsAsync(ct))
                              .Query(new SearchOrdersQuery.Builder()
                                     .Filter(new SearchOrdersFilter.Builder()
                                             .StateFilter(new SearchOrdersStateFilter(new[] { "COMPLETED" }))
                                             .DateTimeFilter(new SearchOrdersDateTimeFilter.Builder()
                                                             .ClosedAt(new TimeRange.Builder()
                                                                       .StartAt(SquareTransactionService
                                                                                    .FormatDateTime(start))
                                                                       .EndAt(SquareTransactionService
                                                                                  .FormatDateTime(end))
                                                                       .Build())
                                                             .Build())
                                             .Build())
                                     .Sort(new SearchOrdersSort("CLOSED_AT"))
                                     .Build())
                              .Limit(100)
                              .ReturnEntries(false);

                SearchOrdersResponse response = null;
                do
                {
                    var request = builder.Cursor(response?.Cursor).Build();
                    response = await this.squareClient.OrdersApi.SearchOrdersAsync(request, ct);
                    this.logger.LogInformation("Loading Square Transactions - Got {Count} Orders", response.Orders.Count);
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

        private static string FormatDateTime(in DateTime dateTime)
        {
            return dateTime.ToString("s");
        }
    }
}
