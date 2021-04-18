using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FixedCommissions
{
    /// <summary>
    /// Represents the ViewModel for the Fixed Commission Amounts Index page.
    /// </summary>
    public interface IFixedCommissionAmountIndexViewModel
    {
        /// <summary>
        /// Gets the command to populate the ViewModel with data.
        /// </summary>
        ReactiveCommand<Unit, IReadOnlyCollection<IFixedCommissionAmountGridRowViewModel>> LoadData { get; }

        /// <summary>
        /// Gets the collection of all Fixed Commission Amounts for display.
        /// </summary>
        ReadOnlyObservableCollection<IFixedCommissionAmountGridRowViewModel> Rows { get; }
    }
}
