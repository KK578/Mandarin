using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Database.Extensions;
using Mandarin.Models.Artists;
using Mandarin.Models.Commissions;
using Mandarin.Models.Common;
using Mandarin.Models.Transactions;
using Mandarin.Services.Square;

namespace Mandarin.Services.Commission
{
    /// <inheritdoc />
    public class CommissionService : ICommissionService
    {
        private readonly IArtistService artistService;
        private readonly ITransactionService transactionService;
        private readonly MandarinDbContext mandarinDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionService"/> class.
        /// </summary>
        /// <param name="artistService">The application service for interacting with stockists.</param>
        /// <param name="transactionService">The transaction service.</param>
        /// <param name="mandarinDbContext">The application database context.</param>
        public CommissionService(IArtistService artistService,
                                 ITransactionService transactionService,
                                 MandarinDbContext mandarinDbContext)
        {
            this.artistService = artistService;
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
        public async Task<IReadOnlyList<RecordOfSales>> GetSalesByArtistForPeriod(DateTime start, DateTime end)
        {
            var transactions = await this.transactionService.GetAllTransactions(start, end).ToList();
            var artists = await this.artistService.GetArtistsForCommissionAsync()
                                    .Where(x => x.StatusCode >= StatusMode.ActiveHidden)
                                    .ToList();

            var aggregateTransactions = transactions
                                        .SelectMany(transaction => transaction.Subtransactions.NullToEmpty())
                                        .GroupBy(subtransaction => (subtransaction.Product?.ProductCode ?? "TLM-Unknown", subtransaction.TransactionUnitPrice))
                                        .Select(ToAggregateSubtransaction)
                                        .ToList();

            return artists.Select(artist => ToArtistSales(artist, aggregateTransactions)).ToList().AsReadOnly();

            static Subtransaction ToAggregateSubtransaction(IEnumerable<Subtransaction> s)
            {
                var list = s.ToList();
                var product = list.First().Product;
                var quantity = list.Sum(y => y.Quantity);
                var subtotal = list.Sum(y => y.Subtotal);
                return new Subtransaction(product, quantity, subtotal);
            }

            RecordOfSales ToArtistSales(Stockist artist, IEnumerable<Subtransaction> subtransactions)
            {
                var artistSubtransactions = subtransactions.Where(x => x.Product.ProductCode.StartsWith(artist.StockistCode)).ToList();
                var rate = decimal.Divide(artist.Commissions.Last().RateGroup.Rate ?? 0, 100);

                if (artistSubtransactions.Count == 0)
                {
                    return new RecordOfSales(artist.StockistCode,
                                             artist.FirstName,
                                             artist.Details.ShortDisplayName,
                                             artist.Details.EmailAddress,
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
                    var sales = artistSubtransactions.Select(x => SaleMapper.FromTransaction(x, rate)).ToList();
                    var subtotal = sales.Sum(x => x.Subtotal);
                    var commission = sales.Sum(x => x.Commission);

                    return new RecordOfSales(artist.StockistCode,
                                             artist.FirstName,
                                             artist.Details.ShortDisplayName,
                                             artist.Details.EmailAddress,
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
