using System;

namespace Mandarin.Transactions.External
{
    /// <summary>
    /// Represents the raw details of a completed transaction, prior to being converted to a <see cref="Mandarin.Transactions.Transaction"/>.
    /// </summary>
    public record ExternalTransaction
    {
        /// <summary>
        /// Gets the unique transaction ID assigned by Square.
        /// </summary>
        public ExternalTransactionId ExternalTransactionId { get; init; }

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
