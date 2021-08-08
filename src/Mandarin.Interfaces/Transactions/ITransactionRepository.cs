using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Transactions.External;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents a repository that can retrieve and update details about <see cref="Mandarin.Transactions.Transaction"/>.
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Gets a list of all <see cref="Mandarin.Transactions.Transaction"/> items.
        /// </summary>
        /// <returns>A <see cref="Task"/> containing a <see cref="IReadOnlyList{T}"/> of all transactions.</returns>
        Task<IReadOnlyList<Transaction>> GetAllTransactionsAsync();

        /// <summary>
        /// Gets a list of all <see cref="Mandarin.Transactions.Transaction"/> items between the given start and end dates.
        /// </summary>
        /// <param name="start">The start datetime to query transactions for.</param>
        /// <param name="end">The end datetime to query transactions for.</param>
        /// <returns>A <see cref="Task"/> containing a <see cref="IReadOnlyList{T}"/> of transactions between the given start and end dates.</returns>
        Task<IReadOnlyList<Transaction>> GetAllTransactionsAsync(DateTime start, DateTime end);

        /// <summary>
        /// Finds the singular <see cref="Mandarin.Transactions.Transaction"/> that matches the provided <see cref="ExternalTransactionId"/>.
        /// If no match is found, returns null.
        /// </summary>
        /// <param name="externalTransactionId">The unique external transaction id to be searched for.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing either the matched <see cref="Mandarin.Transactions.Transaction"/> or null.</returns>
        Task<Transaction> GetTransactionAsync(ExternalTransactionId externalTransactionId);

        /// <summary>
        /// Saves the provided <see cref="Transaction"/>, and returns the new version of the <see cref="Transaction"/> after it
        /// has been saved successfully.
        /// </summary>
        /// <param name="transaction">The <see cref="Transaction"/> to be saved.</param>
        /// <returns>A <see cref="Task"/> containing the new <see cref="Transaction"/> after the operation is complete.</returns>
        Task SaveTransactionAsync(Transaction transaction);
    }
}
