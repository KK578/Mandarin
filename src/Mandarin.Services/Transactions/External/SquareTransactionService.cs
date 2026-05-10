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
using Square.Orders;

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
            SquareTransactionService.Log.Information("Loading Square Transactions - Between {Start} and {End}", start, end);
            return Observable.Create<Order>(SubscribeToOrders);

            async Task SubscribeToOrders(IObserver<Order> o, CancellationToken ct)
            {
                var request = new SearchOrdersRequest
                {
                    LocationIds = await this.ListAllLocationsAsync(ct),
                    Query = new SearchOrdersQuery
                    {
                        Filter = new SearchOrdersFilter
                        {
                            StateFilter = new SearchOrdersStateFilter
                            {
                                States = [OrderState.Completed],
                            },
                            DateTimeFilter = new SearchOrdersDateTimeFilter
                            {
                                CreatedAt = new TimeRange
                                {
                                    StartAt = InstantPattern.General.Format(start.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant()),
                                    EndAt = InstantPattern.General.Format(end.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant()),
                                },
                            },
                        },
                    },
                };

                SearchOrdersResponse response = null;
                do
                {
                    request = request with { Cursor = response?.Cursor };
                    response = await this.squareClient.Orders.SearchAsync(request, cancellationToken: ct);
                    var orders = response.Orders.NullToEmpty().ToList();
                    SquareTransactionService.Log.Information("Loading Square Transactions - Got {Count} Order(s).", orders.Count);
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
            var locations = await this.squareClient.Locations.ListAsync(cancellationToken: ct);
            ct.ThrowIfCancellationRequested();

            if (locations.Locations is not null)
            {
                return locations.Locations.Select(x => x.Id).ToList().AsReadOnly();
            }

            return [];
        }
    }
}
