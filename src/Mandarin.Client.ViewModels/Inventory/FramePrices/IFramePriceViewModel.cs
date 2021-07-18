using System;
using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <summary>
    /// Represents a <see cref="Mandarin.Inventory.FramePrice"/> for display in a grid.
    /// </summary>
    public interface IFramePriceViewModel : IReactiveObject
    {
        /// <summary>
        /// Gets or sets the code of the product associated to the <see cref="Mandarin.Inventory.FramePrice"/>.
        /// </summary>
        string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the product associated to the <see cref="Mandarin.Inventory.FramePrice"/>.
        /// </summary>
        string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the retail price of the product associated to the <see cref="Mandarin.Inventory.FramePrice"/>.
        /// </summary>
        decimal? RetailPrice { get; set; }

        /// <summary>
        /// Gets or sets the commission amount associated to the <see cref="Mandarin.Inventory.FramePrice"/>.
        /// </summary>
        decimal? FramePrice { get; set; }

        /// <summary>
        /// Gets the price that the artist will receive for the sale of the product.
        /// </summary>
        decimal? ArtistPrice { get; }

        /// <summary>
        /// Gets or sets the timestamp at which the frame price should be considered as active.
        /// </summary>
        DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Builds the complete <see cref="Mandarin.Inventory.FramePrice"/> from the current values in the ViewModel.
        /// </summary>
        /// <returns>The fully populated <see cref="Mandarin.Inventory.FramePrice"/>.</returns>
        FramePrice ToFramePrice();
    }
}
