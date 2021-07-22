using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Inventory;

namespace Mandarin.Services.Inventory
{
    /// <summary>
    /// Represents a type that can resolve a products from Square's catalog.
    /// </summary>
    internal interface ISquareProductService
    {
        /// <summary>
        /// Gets a list of all known <see cref="Product"/>.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> containing a <see cref="IReadOnlyList{T}"/> of all products.</returns>
        Task<IReadOnlyList<Product>> GetAllProductsAsync();
    }
}
