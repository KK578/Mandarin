using System;
using System.Reactive.Linq;
using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <inheritdoc cref="IFramePriceViewModel" />
    internal sealed class FramePriceViewModel : ReactiveObject, IFramePriceViewModel
    {
        private readonly ObservableAsPropertyHelper<decimal?> artistPrice;

        private string productCode;
        private string productName;
        private decimal? retailPrice;
        private decimal? framePrice;
        private DateTime? createdAt;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePriceViewModel"/> class.
        /// </summary>
        /// <param name="framePrice">The domain model for the frame price.</param>
        /// <param name="product">The domain model for the Product underlying the frame price.</param>
        public FramePriceViewModel(FramePrice framePrice, Product product)
            : this()
        {
            this.productCode = product.ProductCode.Value;
            this.productName = product.FriendlyString();
            this.createdAt = framePrice.CreatedAt;

            // Must be set via properties to trigger ArtistPrice update
            this.RetailPrice = product.UnitPrice;
            this.FramePrice = framePrice.Amount;

            this.WhenAnyValue(vm => vm.FramePrice)
                .Select(amount => amount == framePrice.Amount ? framePrice.CreatedAt : DateTime.Now)
                .Subscribe(date => this.CreatedAt = date);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePriceViewModel"/> class.
        /// </summary>
        public FramePriceViewModel()
        {
            this.createdAt = DateTime.Now;
            this.artistPrice = this.WhenAnyValue(x => x.RetailPrice, x => x.FramePrice, (retail, frame) => retail - frame)
                                   .ToProperty(this, x => x.ArtistPrice);
        }

        /// <inheritdoc />
        public string ProductCode
        {
            get => this.productCode;
            set => this.RaiseAndSetIfChanged(ref this.productCode, value);
        }

        /// <inheritdoc />
        public string ProductName
        {
            get => this.productName;
            set => this.RaiseAndSetIfChanged(ref this.productName, value);
        }

        /// <inheritdoc />
        public decimal? RetailPrice
        {
            get => this.retailPrice;
            set => this.RaiseAndSetIfChanged(ref this.retailPrice, value);
        }

        /// <inheritdoc />
        public decimal? FramePrice
        {
            get => this.framePrice;
            set => this.RaiseAndSetIfChanged(ref this.framePrice, value);
        }

        /// <inheritdoc/>
        public decimal? ArtistPrice => this.artistPrice.Value;

        /// <inheritdoc/>
        public DateTime? CreatedAt
        {
            get => this.createdAt;
            set => this.RaiseAndSetIfChanged(ref this.createdAt, value);
        }

        /// <inheritdoc/>
        public FramePrice ToFramePrice()
        {
            return new FramePrice
            {
                ProductCode = Mandarin.Inventory.ProductCode.Of(this.productCode),
                Amount = this.FramePrice ?? throw new InvalidOperationException("No frame price has been set."),
                CreatedAt = this.CreatedAt ?? DateTime.Now,
            };
        }
    }
}
