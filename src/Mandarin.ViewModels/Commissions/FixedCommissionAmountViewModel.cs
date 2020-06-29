namespace Mandarin.ViewModels.Commissions
{
    /// <inheritdoc cref="Mandarin.ViewModels.Commissions.IFixedCommissionAmountViewModel" />
    internal sealed class FixedCommissionAmountViewModel : ViewModelBase, IFixedCommissionAmountViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionAmountViewModel"/> class.
        /// </summary>
        public FixedCommissionAmountViewModel()
        {
        }

        /// <inheritdoc/>
        public string ProductCode { get; set; }

        /// <inheritdoc/>
        public decimal Amount { get; set; }
    }
}
