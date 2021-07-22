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
        Task<IReadOnlyList<Product>> GetAllProductsAsync();

        /// <summary>
        /// Finds the singular product that matches the provided product ID.
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="productId">The unique product ID assigned by Square to be searched for.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched product or null.</returns>
        Task<Product> GetProductAsync(ProductId productId);

        /// <summary>
        /// Finds the singular product that matches the provided product code.
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="productCode">The unique product code to be searched for.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched product or null.</returns>
        Task<Product> GetProductAsync(ProductCode productCode);

        /// <summary>
        /// Finds the singular product that matches the provided product name.
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="productName">The name of the product to be searched for.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched product or null.</returns>
        Task<Product> GetProductAsync(ProductName productName);

        /// <summary>
        /// Saves the provided <see cref="Product"/>, and returns the new version of the <see cref="Product"/> after it
        /// has been saved successfully.
        /// </summary>
        /// <param name="product">The <see cref="Product"/> to be saved.</param>
        /// <returns>A <see cref="Task"/> containing the new <see cref="Product"/> after the operation is complete.</returns>
        Task<Product> SaveProductAsync(Product product);
    }
}
