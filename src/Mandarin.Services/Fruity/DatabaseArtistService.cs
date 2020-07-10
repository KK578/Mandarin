using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Mandarin.Configuration;
using Mandarin.Models.Artists;
using Mandarin.Services.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Mandarin.Services.Fruity
{
    /// <inheritdoc />
    internal sealed class DatabaseArtistService : IArtistService
    {
        private readonly MandarinDbContext mandarinDbContext;
        private readonly IOptions<MandarinConfiguration> mandarinConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseArtistService"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mandarinConfiguration">The application configuration.</param>
        public DatabaseArtistService(MandarinDbContext mandarinDbContext, IOptions<MandarinConfiguration> mandarinConfiguration)
        {
            this.mandarinDbContext = mandarinDbContext;
            this.mandarinConfiguration = mandarinConfiguration;
        }

        /// <inheritdoc/>
        public IObservable<Stockist> GetArtistsForDisplayAsync()
        {
            return this.mandarinDbContext.Stockist
                       .Include(x => x.Details)
                       .OrderBy(x => x.StockistCode)
                       .Where(x => x.StatusCode == "ACTIVE")
                       .ToObservable();
        }

        /// <inheritdoc/>
        public IObservable<Stockist> GetArtistsForCommissionAsync()
        {
            var artists = this.mandarinDbContext.Stockist
                              .Include(x => x.Details)
                              .Include(x => x.Commissions).ThenInclude(x => x.RateGroup)
                              .Where(x => x.StatusCode == "ACTIVE")
                              .ToObservable();
            var additionalArtists = this.GetAdditionalArtistDetails();
            return artists.Merge(additionalArtists)
                          .ToList()
                          .Select(x => x.OrderBy(y => y.StockistCode).ToList().AsReadOnly())
                          .SelectMany(x => x);
        }

        private IObservable<Stockist> GetAdditionalArtistDetails()
        {
            var dtos = JArray.FromObject(this.mandarinConfiguration.Value.AdditionalStockists).ToObject<List<ArtistDto>>();
            return dtos != null ? dtos.Select(ArtistMapper.ConvertToModel).ToObservable() : Observable.Empty<Stockist>();
        }
    }
}
