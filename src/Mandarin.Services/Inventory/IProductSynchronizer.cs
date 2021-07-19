using System.Threading.Tasks;
using Mandarin.Inventory;

namespace Mandarin.Services.Inventory
{
    /// <summary>
    /// Represents a type that synchronises external data sources of <see cref="Product"/> with internal data sources.
    /// </summary>
    internal interface IProductSynchronizer
    {
        /// <summary>
        /// Update the underlying repository with the current state of products.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SynchroniseRepositoryAsync();
    }
}
