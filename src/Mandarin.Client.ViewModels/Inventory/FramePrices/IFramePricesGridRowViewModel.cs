namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <summary>
    /// Represents a <see cref="Mandarin.Inventory.FramePrice"/> for display in a grid.
    /// </summary>
    public interface IFramePricesGridRowViewModel
    {
        /// <summary>
        /// Gets the code of the product associated to the <see cref="Mandarin.Inventory.FramePrice"/>.
        /// </summary>
        string ProductCode { get; }

        /// <summary>
        /// Gets the name of the product associated to the <see cref="Mandarin.Inventory.FramePrice"/>.
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// Gets the retail price of the product associated to the <see cref="Mandarin.Inventory.FramePrice"/>.
        /// </summary>
        decimal? RetailPrice { get; }

        /// <summary>
        /// Gets the commission amount associated to the <see cref="Mandarin.Inventory.FramePrice"/>.
        /// </summary>
        decimal FramePrice { get; }

        /// <summary>
        /// Gets the price that the artist will receive for the sale of the product.
        /// </summary>
        decimal? ArtistPrice { get; }
    }
}
