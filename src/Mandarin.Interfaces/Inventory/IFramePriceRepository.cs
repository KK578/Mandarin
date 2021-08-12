using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a repository that can retrieve and update details about <see cref="FramePrice"/>.
    /// </summary>
    public interface IFramePriceRepository
    {
        /// <summary>
        /// Gets the <see cref="FramePrice"/> corresponding to the given product code.
        /// </summary>
        /// <param name="productCode">The product code to search for.</param>
        /// <param name="activeSince">The timestamp at which the frame price must be active at.</param>
        /// <returns>A <see cref="Task"/> containing the <see cref="FramePrice"/> for the given product code.</returns>
        Task<FramePrice> GetByProductCodeAsync(ProductCode productCode, Instant activeSince);

        /// <summary>
        /// Gets the list of all <see cref="FramePrice"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> containing a <see cref="IReadOnlyList{T}"/> of all <see cref="FramePrice"/>.</returns>
        Task<IReadOnlyList<FramePrice>> GetAllAsync();

        /// <summary>
        /// Saves the provided <see cref="FramePrice"/>, and returns the new version of the <see cref="FramePrice"/> after it
        /// has been saved successfully.
        /// </summary>
        /// <param name="framePrice">The <see cref="FramePrice"/> to be saved.</param>
        /// <returns>A <see cref="Task"/> containing the new <see cref="FramePrice"/> after the operation is complete.</returns>
        Task<FramePrice> SaveAsync(FramePrice framePrice);

        /// <summary>
        /// Deletes the <see cref="FramePrice"/> for the given product code if it exists.
        /// </summary>
        /// <param name="productCode">The product code of the <see cref="FramePrice"/> to be deleted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteByProductCodeAsync(ProductCode productCode);
    }
}
