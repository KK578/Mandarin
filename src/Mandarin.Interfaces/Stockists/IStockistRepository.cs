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
        /// Gets the specified artist by their stockist id.
        /// </summary>
        /// <param name="stockistId">The stockist id of the stockist.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous fetch operation of the requested stockist.</returns>
        Task<Stockist> GetStockistByCode(string stockistId);

        /// <summary>
        /// Gets a list of all known stockists.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> containing a <see cref="IReadOnlyList{T}"/> of all stockists.</returns>
        Task<IReadOnlyList<Stockist>> GetAllStockists();

        /// <summary>
        /// Saves the provided stockist details.
        /// This method will automatically detect if the stockist already exists and insert or update accordingly.
        /// </summary>
        /// <param name="stockist">The stockist to be saved.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing the newly saved stockist's database id.</returns>
        Task<int> SaveStockistAsync(Stockist stockist);
    }
}
