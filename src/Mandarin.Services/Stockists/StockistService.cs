using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bashi.Core.Extensions;
using Mandarin.Commissions;
using Mandarin.Stockists;
using Serilog;

namespace Mandarin.Services.Stockists
{
    /// <inheritdoc />
    internal sealed class StockistService : IStockistService
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<StockistService>();

        private readonly IStockistRepository stockistRepository;
        private readonly ICommissionRepository commissionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistService"/> class.
        /// </summary>
        /// <param name="stockistRepository">The application repository for interacting with stockists.</param>
        /// <param name="commissionRepository">The application repository for interacting with commissions.</param>
        public StockistService(IStockistRepository stockistRepository, ICommissionRepository commissionRepository)
        {
            this.stockistRepository = stockistRepository;
            this.commissionRepository = commissionRepository;
        }

        /// <inheritdoc/>
        public async Task<Stockist> GetStockistByCodeAsync(StockistCode stockistCode)
        {
            StockistService.Log.Debug("Fetching stockist '{StockistCode}'.", stockistCode);
            try
            {
                // TODO: Review when Commission is actually populated.
                var stockist = await this.stockistRepository.GetStockistAsync(stockistCode);
                var commission = await this.commissionRepository.GetCommissionByStockist(stockist.StockistId);
                StockistService.Log.Information("Successfully fetched stockist ({@Stockist}).", stockist);
                return stockist with { Commission = commission };
            }
            catch (Exception ex)
            {
                StockistService.Log.Error(ex, "Failed to fetch stockist (StockistCode={StockistCode}).", stockistCode);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Stockist>> GetStockistsAsync()
        {
            StockistService.Log.Debug("Fetching all stockists.");
            try
            {
                // TODO: Review when Commission is actually populated.
                var stockists = await this.stockistRepository.GetAllStockistsAsync();
                var updatedStockists = await Task.WhenAll(stockists.Select(async stockist =>
                {
                    var commission = await this.commissionRepository.GetCommissionByStockist(stockist.StockistId);
                    return stockist with { Commission = commission };
                }));

                StockistService.Log.Information("Successfully fetched {Count} stockist(s).", stockists.Count);
                return updatedStockists.AsReadOnlyList();
            }
            catch (Exception ex)
            {
                StockistService.Log.Error(ex, "Failed to fetch all stockists.");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task SaveStockistAsync(Stockist stockist)
        {
            StockistService.Log.Information("Saving stockist: {@Stockist}", stockist);
            try
            {
                // TODO: Transactionality is broken here, there should be one transaction shared across both Stockist and Commission repositories.
                stockist = await this.stockistRepository.SaveStockistAsync(stockist);
                await this.commissionRepository.SaveCommissionAsync(stockist.StockistId, stockist.Commission);
                StockistService.Log.Information("Successfully saved stockist {StockistCode}", stockist.StockistCode);
            }
            catch (Exception ex)
            {
                StockistService.Log.Error(ex, "Failed to save the stockist {@Stockist}.", stockist);
                throw;
            }
        }
    }
}
