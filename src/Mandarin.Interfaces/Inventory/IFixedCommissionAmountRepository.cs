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
        Task<FixedCommissionAmount> GetFixedCommissionAmountByProductCode(string productCode);

        /// <summary>
        /// Saves the provided <see cref="FixedCommissionAmount"/>, and returns the new version of the <see cref="FixedCommissionAmount"/> after it
        /// has been saved successfully.
        /// </summary>
        /// <param name="fixedCommissionAmount">The <see cref="FixedCommissionAmount"/> to be saved.</param>
        /// <returns>A <see cref="Task"/> containing the new <see cref="FixedCommissionAmount"/> after the operation is complete.</returns>
        Task<FixedCommissionAmount> SaveFixedCommissionAmountAsync(FixedCommissionAmount fixedCommissionAmount);
    }
}
