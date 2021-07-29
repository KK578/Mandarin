using System;
using System.Collections.Generic;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents the details of a completed transaction, with details for the individual products sold within it.
    /// </summary>
    public record Transaction
    {
        /// <summary>
        /// Gets the unique transaction ID assigned by Square for this transaction.
        /// </summary>
        public TransactionId TransactionId { get; init; }

        /// <summary>
        /// Gets the total monetary amount for this transaction.
        /// </summary>
        public decimal TotalAmount { get; init; }

        /// <summary>
        /// Gets the time that this transaction was last updated.
        /// </summary>
        public DateTime Timestamp { get; init; }

        /// <summary>
        /// Gets the list of all subtransactions that form this transaction.
        /// </summary>
        public IReadOnlyList<Subtransaction> Subtransactions { get; init; }
    }
}
