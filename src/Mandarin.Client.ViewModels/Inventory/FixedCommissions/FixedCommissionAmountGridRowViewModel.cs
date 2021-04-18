using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FixedCommissions
{
    /// <inheritdoc cref="IFixedCommissionAmountGridRowViewModel" />
    internal sealed class FixedCommissionAmountGridRowViewModel : ReactiveObject, IFixedCommissionAmountGridRowViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionAmountGridRowViewModel"/> class.
        /// </summary>
        /// <param name="fixedCommissionAmount">The domain model for the Fixed Commission Amount.</param>
        public FixedCommissionAmountGridRowViewModel(FixedCommissionAmount fixedCommissionAmount)
        {
            this.ProductCode = fixedCommissionAmount.ProductCode;
            this.Amount = fixedCommissionAmount.Amount;
        }

        /// <inheritdoc/>
        public string ProductCode { get; }

        /// <inheritdoc/>
        public decimal Amount { get; }
    }
}
