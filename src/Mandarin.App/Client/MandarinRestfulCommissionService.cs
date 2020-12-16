using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Services;

namespace Mandarin.App.Client
{
    /// <inheritdoc />
    internal sealed class MandarinRestfulCommissionService : ICommissionService
    {
        private readonly MandarinHttpClient mandarinHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinRestfulCommissionService"/> class.
        /// </summary>
        /// <param name="mandarinHttpClient">The Mandarin API <see cref="mandarinHttpClient"/>.</param>
        public MandarinRestfulCommissionService(MandarinHttpClient mandarinHttpClient)
        {
            this.mandarinHttpClient = mandarinHttpClient;
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<CommissionRateGroup>> GetCommissionRateGroups()
        {
            return this.mandarinHttpClient.GetAsync<IReadOnlyList<CommissionRateGroup>>("/api/commissions/rates");
        }

        /// <inheritdoc/>
        public IObservable<ArtistSales> GetSalesByArtistForPeriod(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
    }
}
