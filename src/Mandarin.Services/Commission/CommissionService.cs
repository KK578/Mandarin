using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bashi.Core.Extensions;
using Mandarin.Commissions;
using Mandarin.Common;
using Mandarin.Inventory;
using Mandarin.Stockists;
using Mandarin.Transactions;
using NodaTime;

namespace Mandarin.Services.Commission
{
    /// <inheritdoc />
    public class CommissionService : ICommissionService
    {
        private readonly IStockistService stockistService;
        private readonly ITransactionRepository transactionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionService"/> class.
        /// </summary>
        /// <param name="stockistService">The application service for interacting with stockists.</param>
        /// <param name="transactionRepository">The transaction service.</param>
        public CommissionService(IStockistService stockistService, ITransactionRepository transactionRepository)
        {
            this.stockistService = stockistService;
            this.transactionRepository = transactionRepository;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RecordOfSales>> GetRecordOfSalesAsync(DateInterval interval)
        {
            var transactions = await this.transactionRepository.GetAllTransactionsAsync(interval);
            var aggregateTransactions = transactions
                                        .SelectMany(transaction => transaction.Subtransactions.NullToEmpty())
                                        .GroupBy(subtransaction => (subtransaction.Product?.ProductCode ?? ProductCode.Of("TLM-Unknown"), TransactionUnitPrice: subtransaction.UnitPrice))
                                        .Select(ToAggregateSubtransaction)
                                        .ToList();

            var allStockists = await this.stockistService.GetStockistsAsync();
            var stockists = allStockists.Where(x => x.StatusCode >= StatusMode.ActiveHidden).ToList();

            return stockists.Select(s => ToRecordOfSales(s, aggregateTransactions)).ToList().AsReadOnly();

            static Subtransaction ToAggregateSubtransaction(IEnumerable<Subtransaction> s)
            {
                var list = s.ToList();

                return new Subtransaction
                {
                    Product = list.First().Product,
                    Quantity = list.Sum(y => y.Quantity),
                    UnitPrice = list.First().UnitPrice,
                };
            }

            RecordOfSales ToRecordOfSales(Stockist stockist, IEnumerable<Subtransaction> subtransactions)
            {
                var stockistsSubtransactions = subtransactions.Where(x => x.Product.ProductCode.Value.StartsWith(stockist.StockistCode.Value)).ToList();
                var rate = decimal.Divide(stockist.Commission.Rate, 100);

                if (stockistsSubtransactions.Count == 0)
                {
                    return new RecordOfSales
                    {
                        StockistCode = stockist.StockistCode.Value,
                        FirstName = stockist.Details.FirstName,
                        Name = stockist.Details.DisplayName,
                        EmailAddress = stockist.Details.EmailAddress,
                        CustomMessage = string.Empty,
                        StartDate = interval.Start,
                        EndDate = interval.End,
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
                        StockistCode = stockist.StockistCode.Value,
                        FirstName = stockist.Details.FirstName,
                        Name = stockist.Details.DisplayName,
                        EmailAddress = stockist.Details.EmailAddress,
                        CustomMessage = string.Empty,
                        StartDate = interval.Start,
                        EndDate = interval.End,
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
