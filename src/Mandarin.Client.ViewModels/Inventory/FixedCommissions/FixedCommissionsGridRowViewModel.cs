using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FixedCommissions
{
    /// <inheritdoc cref="IFixedCommissionsGridRowViewModel" />
    internal sealed class FixedCommissionsGridRowViewModel : ReactiveObject, IFixedCommissionsGridRowViewModel
    {
        private readonly FixedCommissionAmount fixedCommissionAmount;
        private readonly Product product;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionsGridRowViewModel"/> class.
        /// </summary>
        /// <param name="fixedCommissionAmount">The domain model for the Fixed Commission Amount.</param>
        /// <param name="product">The domain model for the Product underlying the Fixed Commission Amount.</param>
        public FixedCommissionsGridRowViewModel(FixedCommissionAmount fixedCommissionAmount, Product product)
        {
            this.fixedCommissionAmount = fixedCommissionAmount;
            this.product = product;
        }

        /// <inheritdoc/>
        public string ProductCode => this.product.ProductCode;

        /// <inheritdoc/>
        public string ProductName => this.product.ProductName;

        /// <inheritdoc/>
        public decimal? RetailPrice => this.product?.UnitPrice;

        /// <inheritdoc/>
        public decimal FramePrice => this.fixedCommissionAmount.Amount;

        /// <inheritdoc/>
        public decimal? ArtistPrice => this.RetailPrice - this.FramePrice;
    }
}
