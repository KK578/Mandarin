using System;
using Square.Models;
using Transaction = Mandarin.Transactions.Transaction;

namespace Mandarin.Services.Transactions
{
    /// <summary>
    /// Represents the converter between Square DTOs to Domain models.
    /// </summary>
    internal interface ITransactionMapper
    {
        /// <summary>
        /// Maps the Square <see cref="Order"/> object to an observable sequence of <see cref="Transaction"/>.
        /// </summary>
        /// <param name="order">The order to be mapped.</param>
        /// <returns>An observable sequence of transactions found in the order.</returns>
        IObservable<Transaction> MapToTransaction(Order order);
    }
}
