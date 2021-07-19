using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a repository that can retrieve and update details about <see cref="Product"/>.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Gets the list of all <see cref="Product"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> containing a <see cref="IReadOnlyList{T}"/> of all <see cref="Product"/>.</returns>
        Task<IReadOnlyList<Product>> GetAllAsync();

        /// <summary>
        /// Saves the provided <see cref="Product"/>, and returns the new version of the <see cref="Product"/> after it
        /// has been saved successfully.
        /// </summary>
        /// <param name="product">The <see cref="Product"/> to be saved.</param>
        /// <returns>A <see cref="Task"/> containing the new <see cref="Product"/> after the operation is complete.</returns>
        Task<Product> SaveAsync(Product product);
    }
}
