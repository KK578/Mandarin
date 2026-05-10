using System;
using NodaTime;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents the summary of transactions for a given Date.
    /// </summary>
    public record TransactionSummary
    {
        /// <summary>
        /// Gets the date this summary is associated to.
        /// </summary>
        public LocalDate Date { get; init; }

        /// <summary>
        /// Gets the total revenue amount for this date in GBP.
        /// </summary>
        public decimal Revenue { get; init; }

        /// <summary>
        /// Gets the total refund amount for this date in GBP.
        /// </summary>
        public decimal Refund { get; init; }
    }
}
