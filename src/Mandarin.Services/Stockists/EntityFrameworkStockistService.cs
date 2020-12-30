using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Models.Stockists;
using Microsoft.EntityFrameworkCore;

namespace Mandarin.Services.Stockists
{
    /// <inheritdoc />
    internal sealed class EntityFrameworkStockistService : IStockistService
    {
        private readonly MandarinDbContext mandarinDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkStockistService"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        public EntityFrameworkStockistService(MandarinDbContext mandarinDbContext)
        {
            this.mandarinDbContext = mandarinDbContext;
        }

        /// <inheritdoc/>
        public IObservable<Stockist> GetStockistsAsync()
        {
            return this.mandarinDbContext.Stockist
                       .Include(x => x.Details)
                       .Include(x => x.Commissions).ThenInclude(x => x.RateGroup)
                       .OrderBy(x => x.StockistCode)
                       .ToObservable();
        }

        /// <inheritdoc/>
        public async Task SaveStockistAsync(Stockist stockist)
        {
            this.mandarinDbContext.Stockist.Update(stockist);
            await this.mandarinDbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<Stockist> GetStockistByCodeAsync(string stockistCode)
        {
            var stockist = await this.mandarinDbContext.Stockist
                       .Include(x => x.Details)
                       .Include(x => x.Commissions)
                       .ThenInclude(x => x.RateGroup)
                       .FirstOrDefaultAsync(x => x.StockistCode == stockistCode);
            await this.mandarinDbContext.Entry(stockist).ReloadAsync();
            return stockist;
        }
    }
}
