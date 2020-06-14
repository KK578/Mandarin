using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandarin.Models.Transactions
{
    /// <summary>
    /// Represents a the details of a completed transaction, with details for the individual products sold within it.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class.
        /// </summary>
        /// <param name="squareId">The unique transaction id assigned by Square for this transaction.</param>
        /// <param name="totalAmount">The total monetary amount for this transaction.</param>
        /// <param name="timestamp">The timestamp at which this transaction was last updated.</param>
        /// <param name="insertedBy">The name of the user who created the transaction.</param>
        /// <param name="subtransactions">The collection of subtransactions that form the transaction.</param>
        public Transaction(string squareId,
                           decimal totalAmount,
                           DateTime timestamp,
                           string insertedBy,
                           IEnumerable<Subtransaction> subtransactions)
        {
            this.SquareId = squareId;
            this.TotalAmount = totalAmount;
            this.Timestamp = timestamp;
            this.InsertedBy = insertedBy;
            this.Subtransactions = subtransactions.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the unique transaction ID assigned by Square for this transaction.
        /// </summary>
        public string SquareId { get; }

        /// <summary>
        /// Gets the total monetary amount for this transaction.
        /// </summary>
        public decimal TotalAmount { get; }

        /// <summary>
        /// Gets the time that this transaction was last updated.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Gets the user who created the transaction.
        /// </summary>
        public string InsertedBy { get; }

        /// <summary>
        /// Gets the list of all subtransactions that form this transaction.
        /// </summary>
        public IReadOnlyList<Subtransaction> Subtransactions { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Timestamp}: {this.Subtransactions.Count} Transactions = {this.TotalAmount:C}";
        }
    }
}
