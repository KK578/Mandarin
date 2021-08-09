using System.ComponentModel;
using System.Threading.Tasks;
using NodaTime;

namespace Mandarin.Transactions.External
{
    /// <summary>
    /// Represents a type that synchronises external data sources of <see cref="Transaction"/> with internal data sources.
    /// </summary>
    public interface ITransactionSynchronizer
    {
        /// <summary>
        /// Fetch the set of orders that have occurred since start of yesterday.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task LoadExternalTransactionsInPastDay();

        /// <summary>
        /// Fetch the set of orders that have occurred since 2 months ago from today.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task LoadExternalTransactionsInPast2Months();

        /// <summary>
        /// Fetch the set of orders between the given start and end dates, and enqueue a job to process them.
        /// </summary>
        /// <param name="start">The start date to query transactions for.</param>
        /// <param name="end">The end date to query transactions for.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [DisplayName("ITransactionSynchronizer.LoadExternalTransactions({0} => {1})")]
        Task LoadExternalTransactions(LocalDate start, LocalDate end);

        /// <summary>
        /// Check the given Square Order and update the underlying repository if the transaction differs.
        /// </summary>
        /// <param name="externalTransactionId">The transaction id to be updated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [DisplayName("ITransactionSynchronizer.SynchronizeTransaction({0})")]
        Task SynchronizeTransactionAsync(ExternalTransactionId externalTransactionId);
    }
}
