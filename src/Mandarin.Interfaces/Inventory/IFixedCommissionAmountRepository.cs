using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a repository that can retrieve and update details about <see cref="FixedCommissionAmount"/>.
    /// </summary>
    public interface IFixedCommissionAmountRepository
    {
        /// <summary>
        /// Gets the <see cref="FixedCommissionAmount"/> corresponding to the given product code.
        /// </summary>
        /// <param name="productCode">The product code to search for.</param>
        /// <returns>A <see cref="Task"/> containing the <see cref="FixedCommissionAmount"/> for the given product code.</returns>
        Task<FixedCommissionAmount> GetByProductCodeAsync(string productCode);

        /// <summary>
        /// Gets the list of all <see cref="FixedCommissionAmount"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> containing a <see cref="IReadOnlyList{T}"/> of all <see cref="FixedCommissionAmount"/>.</returns>
        Task<IReadOnlyList<FixedCommissionAmount>> GetAllAsync();

        /// <summary>
        /// Saves the provided <see cref="FixedCommissionAmount"/>, and returns the new version of the <see cref="FixedCommissionAmount"/> after it
        /// has been saved successfully.
        /// </summary>
        /// <param name="fixedCommissionAmount">The <see cref="FixedCommissionAmount"/> to be saved.</param>
        /// <returns>A <see cref="Task"/> containing the new <see cref="FixedCommissionAmount"/> after the operation is complete.</returns>
        Task<FixedCommissionAmount> SaveAsync(FixedCommissionAmount fixedCommissionAmount);

        /// <summary>
        /// Deletes the <see cref="FixedCommissionAmount"/> for the given product code if it exists.
        /// </summary>
        /// <param name="productCode">The product code of the <see cref="FixedCommissionAmount"/> to be deleted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteByProductCodeAsync(string productCode);
    }
}
