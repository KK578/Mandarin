using Mandarin.Inventory;

namespace Mandarin.Client.ViewModels.Inventory.FixedCommissions
{
    /// <summary>
    /// Represents a <see cref="FixedCommissionAmount"/> for display in a grid.
    /// </summary>
    public interface IFixedCommissionsGridRowViewModel
    {
        /// <summary>
        /// Gets the code of the product associated to the Fixed Commission Amount.
        /// </summary>
        string ProductCode { get; }

        /// <summary>
        /// Gets the name of the product associated to the Fixed Commission Amount.
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// Gets the retail price of the product associated to the Fixed Commission Amount.
        /// </summary>
        decimal? RetailPrice { get; }

        /// <summary>
        /// Gets the commission amount associated to the Fixed Commission Amount.
        /// </summary>
        decimal FramePrice { get; }

        /// <summary>
        /// Gets the price that the artist will receive for the sale of the product.
        /// </summary>
        decimal? ArtistPrice { get; }
    }
}
