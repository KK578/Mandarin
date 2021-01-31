using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a service that can retrieve and update <see cref="FixedCommissionAmount"/>.
    /// </summary>
    public interface IFixedCommissionService
    {
        /// <summary>
        /// Gets an observable sequence for all known fixed commission amounts.
        /// </summary>
        /// <returns>An observable sequence containing all known fixed commissions.</returns>
        Task<IReadOnlyList<FixedCommissionAmount>> GetFixedCommissionAsync();

        /// <summary>
        /// Gets the fixed commission amount for the requested product.
        /// If a fixed commission amount does not exist, then the result will be null.
        /// </summary>
        /// <param name="productCode">The product code of to a fixed commission amount for.</param>
        /// <returns>A task representing the asynchronous retrieval of the fixed commission amount.</returns>
        Task<FixedCommissionAmount> GetFixedCommissionAsync(string productCode);

        /// <summary>
        /// Saves all changes made to the <see cref="FixedCommissionAmount"/>. Will automatically detect if it is a new <see cref="FixedCommissionAmount"/>
        /// and save it as a new instance as required.
        /// </summary>
        /// <param name="commission">The new commission to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveFixedCommissionAsync(FixedCommissionAmount commission);

        /// <summary>
        /// Delets an existing fixed commission amount, by it's product code.
        /// </summary>
        /// <param name="productCode">The product code of the commission to be deleted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteFixedCommissionAsync(string productCode);
    }
}
