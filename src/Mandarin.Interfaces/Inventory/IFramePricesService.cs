using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a service that can retrieve and update <see cref="FramePrice"/>.
    /// </summary>
    public interface IFramePricesService
    {
        /// <summary>
        /// Gets a list of all known frame prices.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> containing a <see cref="IReadOnlyList{T}"/> of all frame prices.</returns>
        Task<IReadOnlyList<FramePrice>> GetAllFramePricesAsync();

        /// <summary>
        /// Gets the frame price for the requested product by its product code.
        /// If a frame price does not exist, then the result will be null.
        /// </summary>
        /// <param name="productCode">The product code of the product to search for a frame price.</param>
        /// <param name="transactionTime">The timestamp at which the transaction occurred.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous retrieval of the frame price.</returns>
        Task<FramePrice> GetFramePriceAsync(ProductCode productCode, Instant transactionTime);

        /// <summary>
        /// Saves all changes made to the <see cref="FramePrice"/>. Will automatically detect if it is a new <see cref="FramePrice"/>
        /// and save it as a new instance as required.
        /// </summary>
        /// <param name="commission">The new frame price to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveFramePriceAsync(FramePrice commission);

        /// <summary>
        /// Deletes an existing frame price, by its product code.
        /// </summary>
        /// <param name="productCode">The product code of the product to delete its frame price.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteFramePriceAsync(ProductCode productCode);
    }
}
