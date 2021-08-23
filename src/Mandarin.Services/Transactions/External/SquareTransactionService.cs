using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Core.Extensions;
using NodaTime;
using NodaTime.Text;
using Serilog;
using Square;
using Square.Models;

namespace Mandarin.Services.Transactions.External
{
    /// <inheritdoc />
    internal sealed class SquareTransactionService : ISquareTransactionService
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<SquareTransactionService>();

        private readonly ISquareClient squareClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareTransactionService"/> class.
        /// </summary>
        /// <param name="squareClient">The Square API Client.</param>
        public SquareTransactionService(ISquareClient squareClient)
        {
            this.squareClient = squareClient;
        }

        /// <inheritdoc/>
        public IObservable<Order> GetAllOrders(LocalDate start, LocalDate end)
        {
            Log.Information("Loading Square Transactions - Between {Start} and {End}", start, end);
            return Observable.Create<Order>(SubscribeToOrders);

            async Task SubscribeToOrders(IObserver<Order> o, CancellationToken ct)
            {
                var builder = new SearchOrdersRequest.Builder();
                builder.LocationIds(await this.ListAllLocationsAsync(ct));
                builder.Query(new SearchOrdersQuery.Builder()
                              .Filter(new SearchOrdersFilter.Builder()
                                      .StateFilter(new SearchOrdersStateFilter(new[] { "COMPLETED" }))
                                      .DateTimeFilter(new SearchOrdersDateTimeFilter.Builder()
                                                      .CreatedAt(new TimeRange.Builder()
                                                                .StartAt(InstantPattern.General.Format(start.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant()))
                                                                .EndAt(InstantPattern.General.Format(end.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant()))
                                                                .Build())
                                                      .Build())
                                      .Build())
                              .Build());

                SearchOrdersResponse response = null;
                do
                {
                    var request = builder.Cursor(response?.Cursor).Build();
                    response = await this.squareClient.OrdersApi.SearchOrdersAsync(request, ct);
                    var orders = response.Orders.NullToEmpty().ToList();
                    Log.Information("Loading Square Transactions - Got {Count} Order(s).", orders.Count);
                    foreach (var order in orders)
                    {
                        o.OnNext(order);
                    }
                }
                while (response.Cursor != null);

                o.OnCompleted();
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
