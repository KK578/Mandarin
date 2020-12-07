using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Models.Artists;
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
        public async Task<IReadOnlyList<Stockist>> GetArtistsForCommissionAsync()
        {
            var result = await this.mandarinDbContext.Stockist
                                   .Include(x => x.Details)
                                   .Include(x => x.Commissions)
                                   .ThenInclude(x => x.RateGroup)
                                   .OrderBy(x => x.StockistCode)
                                   .ToListAsync();
            return result.AsReadOnly();
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
        public async Task<Stockist> GetArtistByCodeAsync(string stockistCode)
        {
            var stockist = await this.mandarinDbContext.Stockist
                       .Include(x => x.Details)
                       .Include(x => x.Commissions)
                       .ThenInclude(x => x.RateGroup)
                       .FirstOrDefaultAsync(x => x.StockistCode == stockistCode);

            if (stockist != null)
            {
                await this.mandarinDbContext.Entry(stockist).ReloadAsync();
            }

            return stockist;
        }
    }
}
