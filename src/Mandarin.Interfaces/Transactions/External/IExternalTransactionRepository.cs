using System;
using System.Threading.Tasks;

namespace Mandarin.Transactions.External
{
    /// <summary>
    /// Represents a repository that can retrieve and update details about <see cref="ExternalTransaction"/>.
    /// </summary>
    public interface IExternalTransactionRepository
    {
        /// <summary>
        /// Finds the singular <see cref="ExternalTransaction"/> that matches the provided <see cref="ExternalTransactionId"/>.
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="externalTransactionId">The unique transaction id to be searched for.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched <see cref="ExternalTransaction"/> or null.</returns>
        Task<ExternalTransaction> GetExternalTransactionAsync(ExternalTransactionId externalTransactionId);

        /// <summary>
        /// Finds the singular <see cref="ExternalTransaction"/> that matches the provided <see cref="ExternalTransactionId"/> and last updated time..
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="externalTransactionId">The unique transaction id to be searched for.</param>
        /// <param name="updatedAt">The update time for the transaction.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched <see cref="ExternalTransaction"/> or null.</returns>
        Task<ExternalTransaction> GetExternalTransactionAsync(ExternalTransactionId externalTransactionId, DateTime updatedAt);

        /// <summary>
        /// Saves the provided <see cref="ExternalTransaction"/>, and returns the new version of the
        /// <see cref="ExternalTransaction"/> after it has been saved successfully.
        /// </summary>
        /// <param name="externalTransaction">The <see cref="ExternalTransaction"/> to be saved.</param>
        /// <returns>A <see cref="Task"/> containing the new <see cref="ExternalTransaction"/> after the operation is complete.</returns>
        Task SaveExternalTransactionAsync(ExternalTransaction externalTransaction);
    }
}
