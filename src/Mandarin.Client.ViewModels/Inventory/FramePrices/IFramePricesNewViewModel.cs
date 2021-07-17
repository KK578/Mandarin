using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <summary>
    /// Represents the ViewModel for creating a new <see cref="FramePrice"/>.
    /// </summary>
    public interface IFramePricesNewViewModel : IReactiveObject
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
        /// Gets the command to populate the ViewModel with data.
        /// </summary>
        ReactiveCommand<Unit, IReadOnlyList<Product>> LoadData { get; }

        /// <summary>
        /// Gets the command to save the newly created <see cref="FramePrice"/>.
        /// </summary>
        ReactiveCommand<Unit, Unit> Save { get; }

        /// <summary>
        /// Gets the command to cancel and go back to viewing existing frame prices.
        /// </summary>
        ReactiveCommand<Unit, Unit> Cancel { get; }

        /// <summary>
        /// Gets the collection of all available products for selection.
        /// </summary>
        ReadOnlyObservableCollection<Product> Products { get; }

        /// <summary>
        /// Gets or sets the product to be associated to the new <see cref="FramePrice"/>.
        /// </summary>
        Product SelectedProduct { get; set; }
    }
}
