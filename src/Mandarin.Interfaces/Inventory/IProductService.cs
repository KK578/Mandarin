using System;
using System.Threading.Tasks;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a service that can retrieve details about products and inventory.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Gets an observable sequence of all known products.
        /// </summary>
        /// <returns>An observable sequence containing all known products.</returns>
        IObservable<Product> GetAllProducts();
    }

    /// <summary>
    /// Represents an <see cref="IProductService"/> that is also queryable for a specific product.
    /// </summary>
    public interface IQueryableProductService : IProductService
    {
        /// <summary>
        /// Finds the singular product that matches the provided Square product ID.
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="squareId">The unique product ID assigned by Square to be searched for.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched product or null.</returns>
        Task<Product> GetProductBySquareIdAsync(string squareId);

        /// <summary>
        /// Finds the singular product that matches the provided Mandarin product code.
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="productCode">The unique Mandarin product code to be searched for.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched product or null.</returns>
        Task<Product> GetProductByProductCodeAsync(string productCode);

        /// <summary>
        /// Finds the singular product that matches the provided product name.
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="productName">The name of the product to be searched for.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched product or null.</returns>
        Task<Product> GetProductByNameAsync(string productName);
    }
}
