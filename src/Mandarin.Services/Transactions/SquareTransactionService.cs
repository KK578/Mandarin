using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Core.Extensions;
using Microsoft.Extensions.Logging;
using Square;
using Square.Models;

namespace Mandarin.Services.Transactions
{
    /// <inheritdoc />
    internal sealed class SquareTransactionService : ISquareTransactionService
    {
        private readonly ILogger<SquareTransactionService> logger;
        private readonly ISquareClient squareClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareTransactionService"/> class.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        /// <param name="squareClient">The Square API Client.</param>
        public SquareTransactionService(ILogger<SquareTransactionService> logger, ISquareClient squareClient)
        {
            this.logger = logger;
            this.squareClient = squareClient;
        }

        /// <inheritdoc/>
        public IObservable<Order> GetAllOrders(DateTime start, DateTime end)
        {
            this.logger.LogInformation("Loading Square Transactions - Between {Start} and {End}", start, end);
            return Observable.Create<Order>(SubscribeToOrders)
                             .ToList()
                             .SelectMany(FilterOrders);

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
                                                                .EndAt(end.AddDays(14).ToString("s"))
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
                    var orders = response.Orders.NullToEmpty().ToList();
                    this.logger.LogInformation("Loading Square Transactions - Got {Count} Order(s).", orders.Count);
                    foreach (var order in orders)
                    {
                        o.OnNext(order);
                    }
                }
                while (response.Cursor != null);

                o.OnCompleted();
            }

            IObservable<Order> FilterOrders(IList<Order> sourceOrders)
            {
                var orders = sourceOrders.Where(x => DateTime.Parse(x.ClosedAt) < end).Where(x => x.Returns == null).ToList();
                var orderIds = orders.Select(x => x.Id).ToHashSet();
                var refunds = sourceOrders.Where(x => x.Returns != null).Where(x => x.Returns.Any(r => orderIds.Contains(r.SourceOrderId))).ToList();

                return orders.Concat(refunds).ToObservable();
            }
        }

        private async Task<ReadOnlyCollection<string>> ListAllLocationsAsync(CancellationToken ct)
        {
            var locations = await this.squareClient.LocationsApi.ListLocationsAsync(ct);
            ct.ThrowIfCancellationRequested();
            return locations.Locations.Select(x => x.Id).ToList().AsReadOnly();
        }
    }
}
