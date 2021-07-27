using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Square.Models;

namespace Mandarin.Services.Transactions
{
    /// <summary>
    /// Represents a type that can resolve transactions from Square.
    /// </summary>
    internal interface ISquareTransactionService
    {
        /// <summary>
        /// Gets a list of all known <see cref="Square.Models.Order"/>.
        /// </summary>
        /// <param name="start">The start datetime to query transactions for.</param>
        /// <param name="end">The end datetime to query transactions for.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing a <see cref="IReadOnlyList{T}"/> of all Square orders.</returns>
        IObservable<Order> GetAllOrders(DateTime start, DateTime end);
    }
}
