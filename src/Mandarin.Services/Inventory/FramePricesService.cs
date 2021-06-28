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
        private readonly IFramePriceRepository framePriceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePricesService"/> class.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        /// <param name="framePriceRepository">The application repository for interacting with frame prices.</param>
        public FramePricesService(ILogger<SquareTransactionService> logger, IFramePriceRepository framePriceRepository)
        {
            this.logger = logger;
            this.framePriceRepository = framePriceRepository;
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<FramePrice>> GetAllFramePricesAsync()
        {
            this.logger.LogDebug("Fetching all frame prices.");
            return this.framePriceRepository.GetAllAsync();
        }

        /// <inheritdoc/>
        public Task<FramePrice> GetFramePriceAsync(string productCode)
        {
            this.logger.LogDebug("Fetching frame price for '{ProductCode}'.", productCode);
            return this.framePriceRepository.GetByProductCodeAsync(productCode);
        }

        /// <inheritdoc />
        public Task SaveFramePriceAsync(FramePrice commission)
        {
            this.logger.LogInformation("Saving frame price for '{ProductCode}': {@Commission}", commission.ProductCode, commission);
            return this.framePriceRepository.SaveAsync(commission);
        }

        /// <inheritdoc/>
        public Task DeleteFramePriceAsync(string productCode)
        {
            this.logger.LogInformation("Deleting frame price for '{ProductCode}'.", productCode);
            return this.framePriceRepository.DeleteByProductCodeAsync(productCode);
        }
    }
}
