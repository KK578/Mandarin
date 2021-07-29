using System;
using System.Threading.Tasks;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents a repository that can retrieve and update details about <see cref="Mandarin.Transactions.Transaction"/>.
    /// </summary>
    public interface ITransactionAuditRepository
    {
        /// <summary>
        /// Finds the singular product that matches the provided <see cref="Mandarin.Transactions.TransactionId"/>.
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="transactionId">The unique transaction id to be searched for.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched <see cref="Mandarin.Transactions.TransactionAudit"/> or null.</returns>
        Task<TransactionAudit> GetTransactionAuditAsync(TransactionId transactionId);

        /// <summary>
        /// Finds the singular product that matches the provided <see cref="Mandarin.Transactions.TransactionId"/> and last updated time..
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="transactionId">The unique transaction id to be searched for.</param>
        /// <param name="updatedAt">The update time for the transaction.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched <see cref="Mandarin.Transactions.TransactionAudit"/> or null.</returns>
        Task<TransactionAudit> GetTransactionAuditAsync(TransactionId transactionId, DateTime updatedAt);

        /// <summary>
        /// Saves the provided <see cref="Mandarin.Transactions.Transaction"/>, and returns the new version of the <see cref="Mandarin.Transactions.Transaction"/> after it
        /// has been saved successfully.
        /// </summary>
        /// <param name="transactionAudit">The <see cref="Mandarin.Transactions.TransactionAudit"/> to be saved.</param>
        /// <returns>A <see cref="Task"/> containing the new <see cref="Mandarin.Transactions.TransactionAudit"/> after the operation is complete.</returns>
        Task SaveTransactionAuditAsync(TransactionAudit transactionAudit);
    }
}
