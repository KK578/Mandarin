using System.Threading.Tasks;

namespace Mandarin.Commissions
{
    /// <summary>
    /// Represents a repository that can retrieve and update details about <see cref="Commission"/>.
    /// </summary>
    public interface ICommissionRepository
    {
        /// <summary>
        /// Gets the latest commission for the stockist by their database id.
        /// </summary>
        /// <param name="stockistId">The stockist's database id.</param>
        /// <returns>A <see cref="Task"/> containing the <see cref="Commission"/> for the stockist.</returns>
        Task<Commission> GetCommissionByStockist(int stockistId);

        /// <summary>
        /// Saves the provided <see cref="Commission"/>, and returns the new version of the <see cref="Commission"/> after it has been saved
        /// successfully.
        /// </summary>
        /// <param name="stockistId">The stockist's database id.</param>
        /// <param name="commission">The commission details to be saved.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Commission> SaveCommissionAsync(int stockistId, Commission commission);
    }
}
