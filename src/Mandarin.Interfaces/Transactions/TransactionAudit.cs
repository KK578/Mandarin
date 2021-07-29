using System;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents the raw details of a completed transaction, prior to being converted to a <see cref="Transaction"/>.
    /// </summary>
    public record TransactionAudit
    {
        /// <summary>
        /// Gets the unique transaction ID assigned by Square for this transaction.
        /// </summary>
        public TransactionId TransactionId { get; init; }

        /// <summary>
        /// Gets the time that this transaction was created at.
        /// </summary>
        public DateTime CreatedAt { get; init; }

        /// <summary>
        /// Gets the time that this transaction was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; init; }

        /// <summary>
        /// Gets the raw data string representing the transaction.
        /// </summary>
        public string RawData { get; init; }
    }
}
