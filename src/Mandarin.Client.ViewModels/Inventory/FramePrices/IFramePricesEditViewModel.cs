using System;
using System.Reactive;
using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <summary>
    /// Represents the ViewModel for editing an existing <see cref="FramePrice"/>.
    /// </summary>
    public interface IFramePricesEditViewModel : IReactiveObject
    {
        /// <summary>
        /// Gets a value indicating whether gets whether the ViewModel has finished initialisation.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Gets the command to load the Product by ProductCode and populate the ViewModel with data.
        /// </summary>
        ReactiveCommand<string, Unit> LoadData { get; }

        /// <summary>
        /// Gets the command to save the updated <see cref="FramePrice"/>.
        /// </summary>
        ReactiveCommand<Unit, Unit> Save { get; }

        /// <summary>
        /// Gets the command to cancel and go back to viewing existing frame prices.
        /// </summary>
        ReactiveCommand<Unit, Unit> Cancel { get; }

        /// <summary>
        /// Gets the product associates to the current <see cref="FramePrice"/> to.
        /// </summary>
        Product Product { get; }

        /// <summary>
        /// Gets or sets the frame price to be associated to the selected product.
        /// </summary>
        decimal? FrameAmount { get; set; }

        /// <summary>
        /// Gets or sets the timestamp at which the frame price should be considered as active.
        /// </summary>
        DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets the total cost of the product.
        /// </summary>
        decimal? ProductAmount { get; }

        /// <summary>
        /// Gets the commission that will be paid to stockist on selling the product.
        /// </summary>
        decimal? StockistAmount { get; }
    }
}
