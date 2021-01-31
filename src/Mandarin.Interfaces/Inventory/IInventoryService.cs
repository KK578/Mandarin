using System;
using System.Threading.Tasks;
using Mandarin.Commissions;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a service that can retrieve details about products and inventory.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Inserts a new fixed commission amount.
        /// </summary>
        /// <param name="commission">The new commission to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddFixedCommissionAmount(FixedCommissionAmount commission);

        /// <summary>
        /// Updates an existing fixed commission amount.
        /// </summary>
        /// <param name="commission">The commission to be modified.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UpdateFixedCommissionAmount(FixedCommissionAmount commission);

        /// <summary>
        /// Delets an existing fixed commission amount, by it's product code.
        /// </summary>
        /// <param name="productCode">The product code of the commission to be deleted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteFixedCommissionAmount(string productCode);

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
        /// <param name="productCode">The product code to search for.</param>
        /// <returns>A task representing the asynchronous retrieval of the fixed commission amount.</returns>
        Task<FixedCommissionAmount> GetFixedCommissionAmount(string productCode);

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
