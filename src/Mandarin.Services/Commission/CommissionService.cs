using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Models.Artists;
using Mandarin.Models.Commissions;
using Mandarin.Models.Common;
using Mandarin.Models.Transactions;
using Mandarin.Services.Square;
using Microsoft.EntityFrameworkCore;

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
        /// <param name="artistService">The service that can receive artist details.</param>
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
        public async Task<IReadOnlyList<CommissionRateGroup>> GetCommissionRateGroups()
        {
            var results = await this.mandarinDbContext.CommissionRateGroup
                                    .OrderBy(x => x.Rate)
                                    .ToListAsync();
            return results.AsReadOnly();
        }

        /// <inheritdoc />
        public IObservable<ArtistSales> GetSalesByArtistForPeriod(DateTime start, DateTime end)
        {
            return this.transactionService.GetAllTransactions(start, end)
                       .SelectMany(transaction => transaction.Subtransactions.NullToEmpty())
                       .GroupBy(subtransaction => subtransaction.Product?.ProductCode ?? "TLM-Unknown")
                       .SelectMany(subtransactions => subtransactions.ToList().Select(ToAggregateSubtransaction))
                       .ToList()
                       .Zip(this.artistService.GetArtistsForCommissionAsync()
                                .ToObservable()
                                .SelectMany(x => x)
                                .Where(x => x.StatusCode >= StatusMode.ActiveHidden)
                                .ToList(),
                            (s, a) => (Subtransactions: s, Artists: a))
                       .SelectMany(tuple => tuple.Artists.Select(artist => ToArtistSales(artist, tuple.Subtransactions)));

            Subtransaction ToAggregateSubtransaction(IList<Subtransaction> s)
            {
                var product = s.First().Product;
                var quantity = s.Sum(y => y.Quantity);
                var subtotal = s.Sum(y => y.Subtotal);
                return new Subtransaction(product, quantity, subtotal);
            }

            ArtistSales ToArtistSales(Stockist artist, IList<Subtransaction> subtransactions)
            {
                var artistSubtransactions = subtransactions.Where(x => x.Product.ProductCode.StartsWith(artist.StockistCode)).ToList();
                var rate = decimal.Divide(artist.Commissions.Last().RateGroup.Rate ?? 0, 100);

                if (artistSubtransactions.Count == 0)
                {
                    return new ArtistSales(artist.StockistCode,
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

                    return new ArtistSales(artist.StockistCode,
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
