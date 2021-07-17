using System.Reactive;
using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <summary>
    /// Represents the ViewModel for editing an existing <see cref="Mandarin.Inventory.FramePrice"/>.
    /// </summary>
    public interface IFramePricesEditViewModel : IReactiveObject
    {
        /// <summary>
        /// Gets a value indicating whether gets whether the ViewModel has finished initialisation.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Gets the updatable ViewModel representing a <see cref="Mandarin.Inventory.FramePrice"/>.
        /// </summary>
        IFramePriceViewModel FramePrice { get; }

        /// <summary>
        /// Gets the command to load the Product by ProductCode and populate the ViewModel with data.
        /// </summary>
        ReactiveCommand<ProductCode, Unit> LoadData { get; }

        /// <summary>
        /// Gets the command to save the updated <see cref="Mandarin.Inventory.FramePrice"/>.
        /// </summary>
        ReactiveCommand<Unit, Unit> Save { get; }

        /// <summary>
        /// Gets the command to cancel and go back to viewing existing frame prices.
        /// </summary>
        ReactiveCommand<Unit, Unit> Cancel { get; }
    }
}
