using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Mandarin.Models.Artists;
using Mandarin.Models.Commissions;
using Mandarin.Models.Transactions;
using Mandarin.Services.Square;

namespace Mandarin.Services.Commission
{
    /// <inheritdoc />
    public class CommissionService : ICommissionService
    {
        private readonly IArtistService artistService;
        private readonly ITransactionService transactionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionService"/> class.
        /// </summary>
        /// <param name="artistService">The Artist service.</param>
        /// <param name="transactionService">The transaction service.</param>
        public CommissionService(IArtistService artistService, ITransactionService transactionService)
        {
            this.artistService = artistService;
            this.transactionService = transactionService;
        }

        /// <inheritdoc />
        public IObservable<ArtistSales> GetSalesByArtistForPeriod(DateTime start, DateTime end)
        {
            return this.transactionService.GetAllTransactions(start, end)
                       .SelectMany(transaction => transaction.Subtransactions.NullToEmpty())
                       .GroupBy(subtransaction => subtransaction.Product?.ProductCode ?? "TLM-Unknown")
                       .SelectMany(subtransactions => subtransactions.ToList().Select(ToAggregateSubtransaction))
                       .ToList()
                       .Zip(Observable.FromAsync(this.artistService.GetArtistDetailsForCommissionAsync), (s, a) => (Subtransactions: s, Artists: a))
                       .SelectMany(tuple => tuple.Artists.ToObservable().Select(artist => ToArtistSales(artist, tuple.Subtransactions)));

            Subtransaction ToAggregateSubtransaction(IList<Subtransaction> s)
            {
                var product = s.First().Product;
                var quantity = s.Sum(y => y.Quantity);
                var subtotal = s.Sum(y => y.Subtotal);
                return new Subtransaction(product, quantity, subtotal);
            }

            ArtistSales ToArtistSales(ArtistDetailsModel artist, IList<Subtransaction> subtransactions)
            {
                var artistSubtransactions = subtransactions.Where(x => x.Product.ProductCode.StartsWith(artist.StockistCode)).ToList();

                if (artistSubtransactions.Count == 0)
                {
                    return new ArtistSales(artist.StockistCode,
                                           artist.Name,
                                           artist.EmailAddress,
                                           string.Empty,
                                           start,
                                           end,
                                           artist.Rate,
                                           null,
                                           0,
                                           0,
                                           0);
                }
                else
                {
                    var sales = artistSubtransactions.Select(x => SaleMapper.FromTransaction(x, artist.Rate, null)).ToList();
                    var subtotal = sales.Sum(x => x.Subtotal);
                    var commission = sales.Sum(x => x.Commission);

                    return new ArtistSales(artist.StockistCode,
                                           artist.Name,
                                           artist.EmailAddress,
                                           string.Empty,
                                           start,
                                           end,
                                           artist.Rate,
                                           sales,
                                           subtotal,
                                           commission,
                                           subtotal + commission);
                }
            }
        }
    }
}
