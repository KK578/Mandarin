using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Mandarin.Services;

namespace Mandarin.App.Client
{
    /// <inheritdoc />
    internal sealed class MandarinRestfulArtistService : IArtistService
    {
        private readonly MandarinHttpClient mandarinHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinRestfulArtistService"/> class.
        /// </summary>
        /// <param name="mandarinHttpClient">The Mandarin API <see cref="mandarinHttpClient"/>.</param>
        public MandarinRestfulArtistService(MandarinHttpClient mandarinHttpClient)
        {
            this.mandarinHttpClient = mandarinHttpClient;
        }

        /// <inheritdoc/>
        public Task<Stockist> GetArtistByCodeAsync(string stockistCode)
        {
            return this.mandarinHttpClient.GetAsync<Stockist>($"api/stockists/{stockistCode}");
        }

        /// <inheritdoc/>
        public IObservable<Stockist> GetArtistsForCommissionAsync()
        {
            return this.mandarinHttpClient.GetAsync<List<Stockist>>("api/stockists").ToObservable().SelectMany(x => x);
        }

        /// <inheritdoc/>
        public Task SaveArtistAsync(Stockist stockist)
        {
            return this.mandarinHttpClient.PostAsync("api/stockists", stockist);
        }
    }
}
