using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Database.Commissions;
using Mandarin.Database.Stockists;
using Mandarin.Models.Stockists;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Stockists
{
    /// <inheritdoc />
    internal sealed class DatabaseStockistService : IStockistService
    {
        private readonly IStockistRepository stockistRepository;
        private readonly ICommissionRepository commissionRepository;
        private readonly ILogger<DatabaseStockistService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseStockistService"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="stockistRepository">The application repository for interacting with stockists.</param>
        /// <param name="commissionRepository">The application repository for interacting with commissions.</param>
        /// <param name="logger">The application logger.</param>
        public DatabaseStockistService(MandarinDbContext mandarinDbContext,
                                       IStockistRepository stockistRepository,
                                       ICommissionRepository commissionRepository,
                                       ILogger<DatabaseStockistService> logger)
        {
            this.stockistRepository = stockistRepository;
            this.commissionRepository = commissionRepository;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Stockist> GetStockistByCodeAsync(string stockistCode)
        {
            this.logger.LogDebug("Fetching stockist '{StockistCode}'.", stockistCode);
            var stockist = await this.stockistRepository.GetStockistByCode(stockistCode);
            var commission = await this.commissionRepository.GetCommissionByStockist(stockist.StockistId);
            if (commission != null)
            {
                stockist.Commissions.Add(commission);
            }

            return stockist;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Stockist>> GetStockistsAsync()
        {
            this.logger.LogInformation("Fetching all stockists.");
            return await this.stockistRepository.GetAllStockists();
        }

        /// <inheritdoc/>
        public async Task SaveStockistAsync(Stockist stockist)
        {
            this.logger.LogInformation("Saving stockist: {@Stockist}", stockist);
            try
            {
                var stockistId = await this.stockistRepository.SaveStockistAsync(stockist);
                await this.commissionRepository.SaveCommissionAsync(stockistId, stockist.Commissions.First());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to save the stockist {@Stockist}.", stockist);
                throw;
            }
        }
    }
}
