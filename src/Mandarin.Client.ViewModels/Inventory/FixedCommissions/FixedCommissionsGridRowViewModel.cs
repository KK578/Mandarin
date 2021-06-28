using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FixedCommissions
{
    /// <inheritdoc cref="IFixedCommissionsGridRowViewModel" />
    internal sealed class FixedCommissionsGridRowViewModel : ReactiveObject, IFixedCommissionsGridRowViewModel
    {
        private readonly FramePrice framePrice;
        private readonly Product product;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionsGridRowViewModel"/> class.
        /// </summary>
        /// <param name="framePrice">The domain model for the Fixed Commission Amount.</param>
        /// <param name="product">The domain model for the Product underlying the Fixed Commission Amount.</param>
        public FixedCommissionsGridRowViewModel(FramePrice framePrice, Product product)
        {
            this.framePrice = framePrice;
            this.product = product;
        }

        /// <inheritdoc/>
        public string ProductCode => this.product.ProductCode;

        /// <inheritdoc/>
        public string ProductName => this.product.ProductName;

        /// <inheritdoc/>
        public decimal? RetailPrice => this.product?.UnitPrice;

        /// <inheritdoc/>
        public decimal FramePrice => this.framePrice.Amount;

        /// <inheritdoc/>
        public decimal? ArtistPrice => this.RetailPrice - this.FramePrice;
    }
}
