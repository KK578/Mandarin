using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Database.Extensions;
using Mandarin.Models.Commissions;
using Mandarin.Models.Common;
using Mandarin.Models.Stockists;
using Mandarin.Models.Transactions;
using Mandarin.Services.Square;

namespace Mandarin.Services.Commission
{
    /// <inheritdoc />
    public class CommissionService : ICommissionService
    {
        private readonly IStockistService stockistService;
        private readonly ITransactionService transactionService;
        private readonly MandarinDbContext mandarinDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionService"/> class.
        /// </summary>
        /// <param name="stockistService">The application service for interacting with stockists.</param>
        /// <param name="transactionService">The transaction service.</param>
        /// <param name="mandarinDbContext">The application database context.</param>
        public CommissionService(IStockistService stockistService,
                                 ITransactionService transactionService,
                                 MandarinDbContext mandarinDbContext)
        {
            this.stockistService = stockistService;
            this.transactionService = transactionService;
            this.mandarinDbContext = mandarinDbContext;
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<CommissionRateGroup>> GetCommissionRateGroupsAsync()
        {
            return this.mandarinDbContext.CommissionRateGroup
                       .OrderBy(x => x.Rate)
                       .ToReadOnlyListAsync();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RecordOfSales>> GetRecordOfSalesForPeriodAsync(DateTime start, DateTime end)
        {
            var transactions = await this.transactionService.GetAllTransactions(start, end).ToList();
            var stockists = await this.stockistService.GetStockistsAsync()
                                      .Where(x => x.StatusCode >= StatusMode.ActiveHidden)
                                      .ToList();

            var aggregateTransactions = transactions
                                        .SelectMany(transaction => transaction.Subtransactions.NullToEmpty())
                                        .GroupBy(subtransaction => (subtransaction.Product?.ProductCode ?? "TLM-Unknown", subtransaction.TransactionUnitPrice))
                                        .Select(ToAggregateSubtransaction)
                                        .ToList();

            return stockists.Select(s => ToRecordOfSales(s, aggregateTransactions)).ToList().AsReadOnly();

            static Subtransaction ToAggregateSubtransaction(IEnumerable<Subtransaction> s)
            {
                var list = s.ToList();
                var product = list.First().Product;
                var quantity = list.Sum(y => y.Quantity);
                var subtotal = list.Sum(y => y.Subtotal);
                return new Subtransaction(product, quantity, subtotal);
            }

            RecordOfSales ToRecordOfSales(Stockist stockist, IEnumerable<Subtransaction> subtransactions)
            {
                var stockistsSubtransactions = subtransactions.Where(x => x.Product.ProductCode.StartsWith(stockist.StockistCode)).ToList();
                var rate = decimal.Divide(stockist.Commissions.Last().RateGroup.Rate ?? 0, 100);

                if (stockistsSubtransactions.Count == 0)
                {
                    return new RecordOfSales(stockist.StockistCode,
                                             stockist.FirstName,
                                             stockist.Details.ShortDisplayName,
                                             stockist.Details.EmailAddress,
                                             string.Empty,
                                             start,
                                             end,
                                             rate,
                                             null,
                                             0,
                                             0,
                                             0);
                }
                else
                {
                    var sales = stockistsSubtransactions.Select(x => SaleMapper.FromTransaction(x, rate)).ToList();
                    var subtotal = sales.Sum(x => x.Subtotal);
                    var commission = sales.Sum(x => x.Commission);

                    return new RecordOfSales(stockist.StockistCode,
                                             stockist.FirstName,
                                             stockist.Details.ShortDisplayName,
                                             stockist.Details.EmailAddress,
                                             string.Empty,
                                             start,
                                             end,
                                             rate,
                                             sales,
                                             subtotal,
                                             commission,
                                             subtotal + commission);
                }
            }
        }
    }
}
