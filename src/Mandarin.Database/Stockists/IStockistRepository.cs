using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Stockists;

namespace Mandarin.Database.Stockists
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
    }
}
