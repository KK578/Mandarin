using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Inventory;
using Mandarin.Services.Transactions;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Inventory
{
    /// <inheritdoc />
    internal sealed class FramePricesService : IFramePricesService
    {
        private readonly ILogger<SquareTransactionService> logger;
        private readonly IFixedCommissionAmountRepository fixedCommissionAmountRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePricesService"/> class.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        /// <param name="fixedCommissionAmountRepository">The application repository for interacting with fixed commission amounts.</param>
        public FramePricesService(ILogger<SquareTransactionService> logger, IFixedCommissionAmountRepository fixedCommissionAmountRepository)
        {
            this.logger = logger;
            this.fixedCommissionAmountRepository = fixedCommissionAmountRepository;
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<FramePrice>> GetAllFramePricesAsync()
        {
            this.logger.LogDebug("Fetching all frame prices.");
            return this.fixedCommissionAmountRepository.GetAllAsync();
        }

        /// <inheritdoc/>
        public Task<FramePrice> GetFramePriceAsync(string productCode)
        {
            this.logger.LogDebug("Fetching frame price for '{ProductCode}'.", productCode);
            return this.fixedCommissionAmountRepository.GetByProductCodeAsync(productCode);
        }

        /// <inheritdoc />
        public Task SaveFramePriceAsync(FramePrice commission)
        {
            this.logger.LogInformation("Saving frame price for '{ProductCode}': {@Commission}", commission.ProductCode, commission);
            return this.fixedCommissionAmountRepository.SaveAsync(commission);
        }

        /// <inheritdoc/>
        public Task DeleteFramePriceAsync(string productCode)
        {
            this.logger.LogInformation("Deleting frame price for '{ProductCode}'.", productCode);
            return this.fixedCommissionAmountRepository.DeleteByProductCodeAsync(productCode);
        }
    }
}
