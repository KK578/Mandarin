using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bashi.Core.Extensions;
using Mandarin.Commissions;
using Mandarin.Stockists;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Stockists
{
    /// <inheritdoc />
    internal sealed class StockistService : IStockistService
    {
        private readonly IStockistRepository stockistRepository;
        private readonly ICommissionRepository commissionRepository;
        private readonly ILogger<StockistService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistService"/> class.
        /// </summary>
        /// <param name="stockistRepository">The application repository for interacting with stockists.</param>
        /// <param name="commissionRepository">The application repository for interacting with commissions.</param>
        /// <param name="logger">The application logger.</param>
        public StockistService(IStockistRepository stockistRepository,
                               ICommissionRepository commissionRepository,
                               ILogger<StockistService> logger)
        {
            this.stockistRepository = stockistRepository;
            this.commissionRepository = commissionRepository;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Stockist> GetStockistByCodeAsync(StockistCode stockistCode)
        {
            this.logger.LogDebug("Fetching stockist '{StockistCode}'.", stockistCode);
            try
            {
                // TODO: Review when Commission is actually populated.
                var stockist = await this.stockistRepository.GetStockistByCode(stockistCode);
                var commission = await this.commissionRepository.GetCommissionByStockist(stockist.StockistId);
                this.logger.LogInformation("Successfully fetched stockist ({@Stockist}).", stockist);
                return stockist with { Commission = commission };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to fetch stockist (StockistCode={StockistCode}).", stockistCode);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Stockist>> GetStockistsAsync()
        {
            this.logger.LogDebug("Fetching all stockists.");
            try
            {
                // TODO: Review when Commission is actually populated.
                var stockists = await this.stockistRepository.GetAllStockists();
                var updatedStockists = await Task.WhenAll(stockists.Select(async stockist =>
                {
                    var commission = await this.commissionRepository.GetCommissionByStockist(stockist.StockistId);
                    return stockist with { Commission = commission };
                }));

                this.logger.LogInformation("Successfully fetched {Count} stockist(s).", stockists.Count);
                return updatedStockists.AsReadOnlyList();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to fetch all stockists.");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task SaveStockistAsync(Stockist stockist)
        {
            this.logger.LogInformation("Saving stockist: {@Stockist}", stockist);
            try
            {
                // TODO: Transactionality is broken here, there should be one transaction shared across both Stockist and Commission repositories.
                stockist = await this.stockistRepository.SaveStockistAsync(stockist);
                await this.commissionRepository.SaveCommissionAsync(stockist.StockistId, stockist.Commission);
                this.logger.LogInformation("Successfully saved stockist {StockistCode}", stockist.StockistCode);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to save the stockist {@Stockist}.", stockist);
                throw;
            }
        }
    }
}
