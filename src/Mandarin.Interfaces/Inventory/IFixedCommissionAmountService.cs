using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a service that can retrieve details about products and inventory.
    /// </summary>
    public interface IFixedCommissionAmountService
    {
        /// <summary>
        /// Gets an observable sequence for all known fixed commission amounts.
        /// </summary>
        /// <returns>An observable sequence containing all known fixed commissions.</returns>
        Task<IReadOnlyList<FixedCommissionAmount>> GetFixedCommissionAmounts();

        /// <summary>
        /// Gets the fixed commission amount for the requested product.
        /// If a fixed commission amount does not exist, then the result will be null.
        /// </summary>
        /// <param name="productCode">The product code of to a fixed commission amount for.</param>
        /// <returns>A task representing the asynchronous retrieval of the fixed commission amount.</returns>
        Task<FixedCommissionAmount> GetFixedCommissionAmount(string productCode);

        /// <summary>
        /// Inserts a new fixed commission amount.
        /// </summary>
        /// <param name="commission">The new commission to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddFixedCommissionAmount(FixedCommissionAmount commission);

        /// <summary>
        /// Updates an existing fixed commission amount.
        /// </summary>
        /// <param name="commission">The commission to be modified.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UpdateFixedCommissionAmount(FixedCommissionAmount commission);

        /// <summary>
        /// Delets an existing fixed commission amount, by it's product code.
        /// </summary>
        /// <param name="productCode">The product code of the commission to be deleted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteFixedCommissionAmount(string productCode);
    }
}
