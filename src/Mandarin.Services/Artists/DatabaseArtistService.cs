using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Models.Artists;
using Mandarin.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace Mandarin.Services.Artists
{
    /// <inheritdoc />
    internal sealed class DatabaseArtistService : IArtistService
    {
        private readonly MandarinDbContext mandarinDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseArtistService"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        public DatabaseArtistService(MandarinDbContext mandarinDbContext)
        {
            this.mandarinDbContext = mandarinDbContext;
        }

        /// <inheritdoc/>
        public IObservable<Stockist> GetArtistsForDisplayAsync()
        {
            return this.mandarinDbContext.Stockist
                       .Include(x => x.Details)
                       .OrderBy(x => x.StockistCode)
                       .Where(x => x.StatusCode == StatusMode.Active)
                       .ToObservable();
        }

        /// <inheritdoc/>
        public IObservable<Stockist> GetArtistsForCommissionAsync()
        {
            return this.mandarinDbContext.Stockist
                       .Include(x => x.Details)
                       .Include(x => x.Commissions).ThenInclude(x => x.RateGroup)
                       .OrderBy(x => x.StockistCode)
                       .ToObservable();
        }

        /// <inheritdoc/>
        public async Task SaveArtistAsync(Stockist stockist)
        {
            var entry = this.mandarinDbContext.Entry(stockist);
            switch (entry.State)
            {
                case EntityState.Added:
                case EntityState.Detached:
                    await this.mandarinDbContext.Stockist.AddAsync(stockist);
                    break;

                case EntityState.Modified:
                    this.mandarinDbContext.Stockist.Update(stockist);
                    break;
            }

            await this.mandarinDbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public Task<Stockist> GetArtistByCodeAsync(string stockistCode)
        {
            return this.mandarinDbContext.Stockist
                       .Include(x => x.Details)
                       .Include(x => x.Commissions)
                       .ThenInclude(x => x.RateGroup)
                       .FirstOrDefaultAsync(x => x.StockistCode == stockistCode);
        }
    }
}
