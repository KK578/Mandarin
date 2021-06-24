using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FixedCommissions
{
    /// <summary>
    /// Represents the ViewModel for the Fixed Commission Amounts Index page.
    /// </summary>
    public interface IFixedCommissionsIndexViewModel : IReactiveObject
    {
        /// <summary>
        /// Gets a value indicating whether gets whether the ViewModel has finished initialisation.
        /// </summary>
        public bool IsLoading { get; }

        /// <summary>
        /// Gets the command to populate the ViewModel with data.
        /// </summary>
        ReactiveCommand<Unit, IReadOnlyCollection<IFixedCommissionsGridRowViewModel>> LoadData { get; }

        /// <summary>
        /// Gets the command to create a new Fixed Commission Amount.
        /// </summary>
        ReactiveCommand<Unit, Unit> CreateNew { get; }

        /// <summary>
        /// Gets the command to edit the selected Fixed Commission Amount.
        /// </summary>
        ReactiveCommand<Unit, Unit> EditSelected { get; }

        /// <summary>
        /// Gets the collection of all Fixed Commission Amounts for display.
        /// </summary>
        ReadOnlyObservableCollection<IFixedCommissionsGridRowViewModel> Rows { get; }

        /// <summary>
        /// Gets or sets the selected Fixed Commission Amount.
        /// </summary>
        IFixedCommissionsGridRowViewModel SelectedRow { get; set; }
    }
}
