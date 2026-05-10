using Mandarin.Transactions;
using NodaTime;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Transactions
{
    /// <summary>
    /// Represents a view model for a <see cref="TransactionSummary"/> that can be used in the UI.
    /// </summary>
    /// <param name="model">The <see cref="TransactionSummary"/> to be handled.</param>
    public class TransactionSummaryViewModel(TransactionSummary model) : ReactiveObject, ITransactionSummaryViewModel
    {
        /// <inheritdoc />
        public LocalDate Date => model.Date;

        /// <inheritdoc />
        public decimal Revenue => model.Revenue;

        /// <inheritdoc />
        public decimal Refund => model.Refund;
    }
}
