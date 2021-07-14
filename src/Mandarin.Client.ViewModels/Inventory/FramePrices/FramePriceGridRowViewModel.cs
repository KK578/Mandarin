using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <inheritdoc cref="IFramePriceGridRowViewModel" />
    internal sealed class FramePriceGridRowViewModel : ReactiveObject, IFramePriceGridRowViewModel
    {
        private readonly FramePrice framePrice;
        private readonly Product product;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePriceGridRowViewModel"/> class.
        /// </summary>
        /// <param name="framePrice">The domain model for the frame price.</param>
        /// <param name="product">The domain model for the Product underlying the frame price.</param>
        public FramePriceGridRowViewModel(FramePrice framePrice, Product product)
        {
            this.framePrice = framePrice;
            this.product = product;
        }

        /// <inheritdoc/>
        public string ProductCode => this.product.ProductCode.Value;

        /// <inheritdoc/>
        public string ProductName => this.product.ProductName.Value;

        /// <inheritdoc/>
        public decimal? RetailPrice => this.product?.UnitPrice;

        /// <inheritdoc/>
        public decimal FramePrice => this.framePrice.Amount;

        /// <inheritdoc/>
        public decimal? ArtistPrice => this.RetailPrice - this.FramePrice;
    }
}
