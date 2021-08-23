using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Inventory;
using Mandarin.Services.Transactions.External;
using NodaTime;
using Serilog;

namespace Mandarin.Services.Inventory
{
    /// <inheritdoc />
    internal sealed class FramePricesService : IFramePricesService
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<SquareTransactionService>();
        private readonly IFramePriceRepository framePriceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePricesService"/> class.
        /// </summary>
        /// <param name="framePriceRepository">The application repository for interacting with frame prices.</param>
        public FramePricesService(IFramePriceRepository framePriceRepository)
        {
            this.framePriceRepository = framePriceRepository;
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<FramePrice>> GetAllFramePricesAsync()
        {
            Log.Debug("Fetching all frame prices.");
            return this.framePriceRepository.GetAllAsync();
        }

        /// <inheritdoc/>
        public Task<FramePrice> GetFramePriceAsync(ProductCode productCode, Instant transactionTime)
        {
            Log.Debug("Fetching frame price for '{ProductCode}' @ '{TransactionTime}'.", productCode, transactionTime);
            return this.framePriceRepository.GetByProductCodeAsync(productCode, transactionTime);
        }

        /// <inheritdoc />
        public Task SaveFramePriceAsync(FramePrice commission)
        {
            Log.Information("Saving frame price for '{ProductCode}': {@Commission}", commission.ProductCode, commission);
            return this.framePriceRepository.SaveAsync(commission);
        }

        /// <inheritdoc/>
        public Task DeleteFramePriceAsync(ProductCode productCode)
        {
            Log.Information("Deleting frame price for '{ProductCode}'.", productCode);
            return this.framePriceRepository.DeleteByProductCodeAsync(productCode);
        }
    }
}
