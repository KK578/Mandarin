using Mandarin.Transactions;
using NodaTime;

namespace Mandarin.Client.ViewModels.Transactions
{
    /// <summary>
    /// Represents a view model for a <see cref="TransactionSummary"/> that can be used in the UI.
    /// </summary>
    public interface ITransactionSummaryViewModel
    {
        /// <summary>
        /// Gets the date for this transaction summary.
        /// </summary>
        LocalDate Date { get; }

        /// <summary>
        /// Gets the total revenue amount for this date in GBP.
        /// </summary>
        decimal Revenue { get; }

        /// <summary>
        /// Gets the total refund amount for this date in GBP.
        /// </summary>
        decimal Refund { get; }
    }
}
