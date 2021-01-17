using System;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents a service that can retrieve details about retail transactions.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Gets an observable sequence containing all retail transactions that occurred between the start and end datetimes.
        /// </summary>
        /// <param name="start">The start datetime to query transactions for.</param>
        /// <param name="end">The end datetime to query transactions for.</param>
        /// <returns>Observable sequence of all transactions between the specified datetimes.</returns>
        IObservable<Transaction> GetAllTransactions(DateTime start, DateTime end);

        /// <summary>
        /// Gets an observable sequence containing all online transactions that occurred between the start and end datetimes.
        /// </summary>
        /// <param name="start">The start datetime to query transactions for.</param>
        /// <param name="end">The end datetime to query transactions for.</param>
        /// <returns>Observable sequence of all online transactions between the specified datetimes.</returns>
        IObservable<Transaction> GetAllOnlineTransactions(DateTime start, DateTime end);
    }
}
