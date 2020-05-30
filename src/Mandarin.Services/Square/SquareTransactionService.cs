using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Mandarin.Models.Inventory;
using Mandarin.Models.Transactions;
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
        private readonly IQueryableInventoryService inventoryService;

        public SquareTransactionService(ILogger<SquareTransactionService> logger,
                                        ISquareClient squareClient,
                                        IQueryableInventoryService inventoryService)
        {
            this.logger = logger;
            this.squareClient = squareClient;
            this.inventoryService = inventoryService;
        }

        public IObservable<Transaction> GetAllTransactions(DateTime start, DateTime end)
        {
            this.logger.LogInformation("Loading Square Transactions - Between {Start} and {End}", start, end);
            return Observable.Create<Order>(SubscribeToOrders)
                             .SelectMany(this.MapToTransaction);

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

        private IObservable<Transaction> MapToTransaction(Order order)
        {
            return this.CreateSubtransactions(order)
                       .Select(subtransactions =>
                       {
                           var transactionId = order.Id;
                           var totalAmount = decimal.Divide(order.TotalMoney?.Amount ?? 0, 100);
                           var timestamp = DateTime.Parse(order.CreatedAt);
                           var insertedBy = order.Source?.Name;
                           return new Transaction(transactionId, totalAmount, timestamp, insertedBy, subtransactions);
                       });
        }

        private IObservable<IList<Subtransaction>> CreateSubtransactions(Order order)
        {
            var lineItems = order.LineItems.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransaction);
            var discounts = order.Discounts.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransactionFromDiscount);
            var returns = order.Returns.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransactionsFromReturn);
            return Observable.Merge(lineItems, discounts, returns).ToList();
        }

        private IObservable<Subtransaction> CreateSubtransaction(OrderLineItem orderLineItem)
        {
            return this.GetProductAsync(orderLineItem.CatalogObjectId, orderLineItem.Name)
                       .ToObservable()
                       .Select(product =>
                       {
                           var quantity = int.Parse(orderLineItem.Quantity);
                           var subTotal = decimal.Divide(orderLineItem.TotalMoney?.Amount ?? 0, 100);
                           return new Subtransaction(product, quantity, subTotal);
                       });
        }

        private IObservable<Subtransaction> CreateSubtransactionFromDiscount(OrderLineItemDiscount orderLineItemDiscount)
        {
            Product product;

            if (orderLineItemDiscount.Name.Contains("macaron", StringComparison.OrdinalIgnoreCase))
            {
                product = new Product("BUN-DCM", "BUN-DCM", "Box of Macarons Discount", "Buy 6 macarons for \"£12.00\"", 0.01m);
            }
            else if (orderLineItemDiscount.Name.Contains("pocky", StringComparison.OrdinalIgnoreCase))
            {
                product = new Product("BUN-DCP", "BUN-DCP", "Box of Pocky Discount", "Discount on buying multiple packs of Pocky.", 0.01m);
            }
            else
            {
                product = new Product("TLM-D", "TLM-D", "Other discounts", "Discounts that aren't tracked.", 0.01m);
            }

            var quantity = (int)(orderLineItemDiscount.AmountMoney.Amount ?? 0);
            var subtransaction = new Subtransaction(product, quantity, orderLineItemDiscount.AmountMoney.Amount ?? 0);
            return Observable.Return(subtransaction);
        }

        private IObservable<Subtransaction> CreateSubtransactionsFromReturn(OrderReturn orderReturn)
        {
            return orderReturn.ReturnLineItems.ToObservable()
                              .SelectMany(async item =>
                              {
                                  var product = await this.GetProductAsync(item.CatalogObjectId, item.Name);
                                  var quantity = -1 * int.Parse(item.Quantity);
                                  var subtotal = decimal.Divide(item.TotalMoney?.Amount ?? 0, 100);
                                  return new Subtransaction(product, quantity, subtotal);
                              });
        }

        private Task<Product> GetProductAsync(string squareId, string name)
        {
            return squareId != null
                ? this.inventoryService.GetProductByIdAsync(squareId)
                : this.inventoryService.GetProductByNameAsync(name);
        }
    }
}
