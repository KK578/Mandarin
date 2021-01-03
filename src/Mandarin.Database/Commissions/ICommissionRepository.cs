using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;

namespace Mandarin.Database.Commissions
{
    /// <summary>
    /// Represents a repository that can retrieve and update details about <see cref="Commission"/>.
    /// </summary>
    public interface ICommissionRepository
    {
        /// <summary>
        /// Gets all currently available commission rate groups.
        /// </summary>
        /// <returns>A <see cref="Task"/> containing a <see cref="IReadOnlyList{T}"/> of all commission rate groups.</returns>
        Task<IReadOnlyList<CommissionRateGroup>> GetCommissionRateGroups();

        /// <summary>
        /// Gets the latest commission for the stockist by their database id.
        /// </summary>
        /// <param name="stockistId">The stockist's database id.</param>
        /// <returns>A <see cref="Task"/> containing the <see cref="Commission"/> for the stockist.</returns>
        Task<Commission> GetCommissionByStockist(int stockistId);

        /// <summary>
        /// Saves the provided commission details.
        /// This method will automatically detect if the commission already exists and insert or update accordingly.
        /// </summary>
        /// <param name="stockistId">The stockist's database id.</param>
        /// <param name="commission">The commission details to be saved.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveCommissionAsync(int stockistId, Commission commission);
    }
}
