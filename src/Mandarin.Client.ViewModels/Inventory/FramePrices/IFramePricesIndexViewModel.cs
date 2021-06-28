using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <summary>
    /// Represents the ViewModel for viewing all existing <see cref="FramePrice"/> instances.
    /// </summary>
    public interface IFramePricesIndexViewModel : IReactiveObject
    {
        /// <summary>
        /// Gets a value indicating whether gets whether the ViewModel has finished initialisation.
        /// </summary>
        public bool IsLoading { get; }

        /// <summary>
        /// Gets the command to populate the ViewModel with data.
        /// </summary>
        ReactiveCommand<Unit, IReadOnlyCollection<IFramePriceGridRowViewModel>> LoadData { get; }

        /// <summary>
        /// Gets the command to create a new <see cref="FramePrice"/>.
        /// </summary>
        ReactiveCommand<Unit, Unit> CreateNew { get; }

        /// <summary>
        /// Gets the command to edit the selected <see cref="FramePrice"/>.
        /// </summary>
        ReactiveCommand<Unit, Unit> EditSelected { get; }

        /// <summary>
        /// Gets the collection of all known <see cref="FramePrice"/> instances.
        /// </summary>
        ReadOnlyObservableCollection<IFramePriceGridRowViewModel> Rows { get; }

        /// <summary>
        /// Gets or sets the selected <see cref="FramePrice"/>.
        /// </summary>
        IFramePriceGridRowViewModel SelectedRow { get; set; }
    }
}
