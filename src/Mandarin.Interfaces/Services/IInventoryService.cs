using System;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Models.Inventory;

namespace Mandarin.Services
{
    /// <summary>
    /// Represents a service that can retrieve details about products and inventory.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Gets an observable sequence for all known fixed commission amounts.
        /// </summary>
        /// <returns>An observable sequence containing all known fixed commissions.</returns>
        IObservable<FixedCommissionAmount> GetFixedCommissionAmounts();

        /// <summary>
        /// Gets an observable sequence of all known products.
        /// </summary>
        /// <returns>An observable sequence containing all known products.</returns>
        IObservable<Product> GetInventory();
    }

    /// <summary>
    /// Represents an <see cref="IInventoryService"/> that is also queryable for a specific product.
    /// </summary>
    public interface IQueryableInventoryService : IInventoryService
    {
        /// <summary>
        /// Gets the fixed commission amount for the requested product.
        /// If a fixed commission amount does not exist, then the result will be null.
        /// </summary>
        /// <param name="product">The product to a fixed commission amount for.</param>
        /// <returns>A task representing the asynchronous retrieval of the fixed commission amount.</returns>
        Task<FixedCommissionAmount> GetFixedCommissionAmount(Product product);

        /// <summary>
        /// Finds the singular product that matches the provided Square product ID.
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="squareId">The unique product ID assigned by Square to be searched for.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched product or null.</returns>
        Task<Product> GetProductByIdAsync(string squareId);

        /// <summary>
        /// Finds the singular product that matches the provided product name.
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="productName">The name of the product to be searched for.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched product or null.</returns>
        Task<Product> GetProductByNameAsync(string productName);
    }
}
