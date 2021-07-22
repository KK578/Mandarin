using System.Threading.Tasks;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a type that synchronises external data sources of <see cref="Product"/> with internal data sources.
    /// </summary>
    public interface IProductSynchronizer
    {
        /// <summary>
        /// Update the underlying repository with the current state of products.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SynchroniseProductsAsync();
    }
}
