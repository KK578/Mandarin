using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents a type that synchronises external data sources of <see cref="Transaction"/> with internal data sources.
    /// </summary>
    public interface ITransactionSynchronizer
    {
        /// <summary>
        /// Fetch the set of orders between the given start and end dates, and enqueue a job to process them.
        /// </summary>
        /// <param name="start">The start datetime to query transactions for.</param>
        /// <param name="end">The end datetime to query transactions for.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [DisplayName("ITransactionSynchronizer.LoadSquareOrders({0} => {1})")]
        Task LoadSquareOrders(DateTime start, DateTime end);

        /// <summary>
        /// Check the given Square Order and update the underlying repository if the transaction differs.
        /// </summary>
        /// <param name="transactionId">The transaction id to be updated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [DisplayName("ITransactionSynchronizer.SynchronizeTransaction({0})")]
        Task SynchronizeTransactionAsync(TransactionId transactionId);
    }
}
