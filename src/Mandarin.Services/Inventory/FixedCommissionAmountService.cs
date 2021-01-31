using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Inventory;
using Mandarin.Services.Transactions;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Inventory
{
    /// <inheritdoc />
    internal sealed class FixedCommissionAmountService : IFixedCommissionAmountService
    {
        private readonly ILogger<SquareTransactionService> logger;
        private readonly IFixedCommissionAmountRepository fixedCommissionAmountRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionAmountService"/> class.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        /// <param name="fixedCommissionAmountRepository">The application repository for interacting with fixed commission amounts.</param>
        public FixedCommissionAmountService(ILogger<SquareTransactionService> logger,
                                            IFixedCommissionAmountRepository fixedCommissionAmountRepository)
        {
            this.logger = logger;
            this.fixedCommissionAmountRepository = fixedCommissionAmountRepository;
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<FixedCommissionAmount>> GetFixedCommissionAmounts()
        {
            this.logger.LogDebug("Fetching all fixed commission amounts.");
            return this.fixedCommissionAmountRepository.GetAllAsync();
        }

        /// <inheritdoc/>
        public Task<FixedCommissionAmount> GetFixedCommissionAmount(string productCode)
        {
            this.logger.LogDebug("Fetching fixed commission amount for '{ProductCode}'.", productCode);
            return this.fixedCommissionAmountRepository.GetByProductCodeAsync(productCode);
        }

        /// <inheritdoc />
        public Task AddFixedCommissionAmount(FixedCommissionAmount commission)
        {
            this.logger.LogInformation("Adding new fixed commission amount for '{ProductCode}': {@Commission}", commission.ProductCode, commission);
            return this.fixedCommissionAmountRepository.SaveAsync(commission);
        }

        /// <inheritdoc/>
        public Task UpdateFixedCommissionAmount(FixedCommissionAmount commission)
        {
            this.logger.LogInformation("Updating fixed commission amount for '{ProductCode}': {@Commission}", commission.ProductCode, commission);
            return this.fixedCommissionAmountRepository.SaveAsync(commission);
        }

        /// <inheritdoc/>
        public Task DeleteFixedCommissionAmount(string productCode)
        {
            this.logger.LogInformation("Deleting fixed commission amount for '{ProductCode}'.", productCode);
            return this.fixedCommissionAmountRepository.DeleteByProductCodeAsync(productCode);
        }
    }
}
