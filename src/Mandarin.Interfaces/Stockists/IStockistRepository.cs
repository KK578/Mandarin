using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mandarin.Stockists
{
    /// <summary>
    /// Represents a repository that can retrieve and update details about <see cref="Stockist"/>.
    /// </summary>
    public interface IStockistRepository
    {
        /// <summary>
        /// Gets the specified artist by their stockist code.
        /// </summary>
        /// <param name="stockistCode">The stockist code of the stockist.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous fetch operation of the requested stockist.</returns>
        Task<Stockist> GetStockistAsync(StockistCode stockistCode);

        /// <summary>
        /// Gets a list of all known stockists.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> containing a <see cref="IReadOnlyList{T}"/> of all stockists.</returns>
        Task<IReadOnlyList<Stockist>> GetAllStockistsAsync();

        /// <summary>
        /// Gets a list of all <see cref="Stockist"/>s who are actively considered for commission.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> containing a <see cref="IReadOnlyList{T}"/> of all active stockists.</returns>
        Task<IReadOnlyList<Stockist>> GetAllActiveStockistsAsync();

        /// <summary>
        /// Saves the provided stockist details.
        /// This method will automatically detect if the stockist already exists and insert or update accordingly.
        /// </summary>
        /// <param name="stockist">The stockist to be saved.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing the newly saved stockist's database id.</returns>
        Task<Stockist> SaveStockistAsync(Stockist stockist);
    }
}
