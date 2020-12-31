using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Database.Extensions;
using Mandarin.Models.Stockists;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Stockists
{
    /// <inheritdoc />
    internal sealed class EntityFrameworkStockistService : IStockistService
    {
        private readonly MandarinDbContext mandarinDbContext;
        private readonly ILogger<EntityFrameworkStockistService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkStockistService"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="logger">The application logger.</param>
        public EntityFrameworkStockistService(MandarinDbContext mandarinDbContext,
                                              ILogger<EntityFrameworkStockistService> logger)
        {
            this.mandarinDbContext = mandarinDbContext;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Stockist> GetStockistByCodeAsync(string stockistCode)
        {
            this.logger.LogDebug("Fetching stockist '{StockistCode}'.", stockistCode);
            var stockist = await this.mandarinDbContext.Stockist
                       .Include(x => x.Details)
                       .Include(x => x.Commissions)
                       .ThenInclude(x => x.RateGroup)
                       .FirstOrDefaultAsync(x => x.StockistCode == stockistCode);
            this.logger.LogInformation("Fetched stockist '{StockistCode}': {Stockist}", stockistCode, stockist);
            await this.mandarinDbContext.Entry(stockist).ReloadAsync();
            return stockist;
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<Stockist>> GetStockistsAsync()
        {
            this.logger.LogInformation("Fetching all stockists.");
            return this.mandarinDbContext.Stockist
                       .Include(x => x.Details)
                       .Include(x => x.Commissions).ThenInclude(x => x.RateGroup)
                       .OrderBy(x => x.StockistCode)
                       .ToReadOnlyListAsync();
        }

        /// <inheritdoc/>
        public async Task SaveStockistAsync(Stockist stockist)
        {
            this.logger.LogInformation("Saving stockist: {Stockist}", stockist);
            this.mandarinDbContext.Stockist.Update(stockist);
            await this.mandarinDbContext.SaveChangesAsync();
        }
    }
}
