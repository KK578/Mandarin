using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Shared
{
    /// <summary>
    /// Represents a ViewModel for viewing all existing instances of <typeparamref name="TRowViewModel"/>.
    /// </summary>
    /// <typeparam name="TRowViewModel">The type of the ViewModel.</typeparam>
    public interface IIndexPageViewModel<TRowViewModel> : IReactiveObject
    {
        /// <summary>
        /// Gets a value indicating whether gets whether the ViewModel has finished initialisation.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Gets the command to populate the ViewModel with data.
        /// </summary>
        ReactiveCommand<Unit, IReadOnlyCollection<TRowViewModel>> LoadData { get; }

        /// <summary>
        /// Gets the command to create a new <typeparamref name="TRowViewModel"/>.
        /// </summary>
        ReactiveCommand<Unit, Unit> CreateNew { get; }

        /// <summary>
        /// Gets the command to edit the selected <typeparamref name="TRowViewModel"/>.
        /// </summary>
        ReactiveCommand<Unit, Unit> EditSelected { get; }

        /// <summary>
        /// Gets the collection of all known <typeparamref name="TRowViewModel"/> instances.
        /// </summary>
        ReadOnlyObservableCollection<TRowViewModel> Rows { get; }

        /// <summary>
        /// Gets or sets the selected <typeparamref name="TRowViewModel"/>.
        /// </summary>
        TRowViewModel SelectedRow { get; set; }
    }
}
