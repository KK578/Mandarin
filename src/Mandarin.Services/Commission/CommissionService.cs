using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.Commissions;
using Mandarin.Common;
using Mandarin.Services.Square;
using Mandarin.Stockists;
using Mandarin.Transactions;

namespace Mandarin.Services.Commission
{
    /// <inheritdoc />
    public class CommissionService : ICommissionService
    {
        private readonly ICommissionRepository commissionRepository;
        private readonly IStockistService stockistService;
        private readonly ITransactionService transactionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionService"/> class.
        /// </summary>
        /// <param name="commissionRepository">The application repository for interacting with commissions.</param>
        /// <param name="stockistService">The application service for interacting with stockists.</param>
        /// <param name="transactionService">The transaction service.</param>
        public CommissionService(ICommissionRepository commissionRepository,
                                 IStockistService stockistService,
                                 ITransactionService transactionService)
        {
            this.commissionRepository = commissionRepository;
            this.stockistService = stockistService;
            this.transactionService = transactionService;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RecordOfSales>> GetRecordOfSalesForPeriodAsync(DateTime start, DateTime end)
        {
            var transactions = await this.transactionService.GetAllTransactions(start, end).ToList();
            var allStockists = await this.stockistService.GetStockistsAsync();
            var stockists = allStockists.Where(x => x.StatusCode >= StatusMode.ActiveHidden).ToList();

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
                var rate = decimal.Divide(stockist.Commission.Rate, 100);

                if (stockistsSubtransactions.Count == 0)
                {
                    return new RecordOfSales
                    {
                        StockistCode = stockist.StockistCode,
                        FirstName = stockist.FirstName,
                        Name = stockist.Details.ShortDisplayName,
                        EmailAddress = stockist.Details.EmailAddress,
                        CustomMessage = string.Empty,
                        StartDate = start,
                        EndDate = end,
                        Rate = rate,
                        Sales = new List<Sale>().AsReadOnly(),
                        Subtotal = 0,
                        CommissionTotal = 0,
                        Total = 0,
                    };
                }
                else
                {
                    var sales = stockistsSubtransactions.Select(x => SaleMapper.FromTransaction(x, rate)).ToList();
                    var subtotal = sales.Sum(x => x.Subtotal);
                    var commission = sales.Sum(x => x.Commission);

                    return new RecordOfSales
                    {
                        StockistCode = stockist.StockistCode,
                        FirstName = stockist.FirstName,
                        Name = stockist.Details.ShortDisplayName,
                        EmailAddress = stockist.Details.EmailAddress,
                        CustomMessage = string.Empty,
                        StartDate = start,
                        EndDate = end,
                        Rate = rate,
                        Sales = sales,
                        Subtotal = subtotal,
                        CommissionTotal = commission,
                        Total = subtotal + commission,
                    };
                }
            }
        }
    }
}
