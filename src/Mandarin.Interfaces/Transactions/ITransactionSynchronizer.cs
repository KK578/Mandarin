using System;
using System.Threading.Tasks;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents a type that synchronises external data sources of <see cref="Transaction"/> with internal data sources.
    /// </summary>
    public interface ITransactionSynchronizer
    {
        /// <summary>
        /// Update the underlying repository with the current state of transactions.
        /// </summary>
        /// <param name="start">The start datetime to query transactions for.</param>
        /// <param name="end">The end datetime to query transactions for.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SynchroniseTransactionsAsync(DateTime start, DateTime end);
    }
}
