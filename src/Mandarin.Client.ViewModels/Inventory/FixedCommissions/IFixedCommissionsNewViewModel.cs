using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FixedCommissions
{
    /// <summary>
    /// Represents the ViewModel for the Fixed Commission Amounts New page.
    /// </summary>
    public interface IFixedCommissionsNewViewModel : IReactiveObject
    {
        /// <summary>
        /// Gets a value indicating whether gets whether the ViewModel has finished initialisation.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Gets the command to populate the ViewModel with data.
        /// </summary>
        ReactiveCommand<Unit, IReadOnlyList<Product>> LoadData { get; }

        /// <summary>
        /// Gets the command to save the newly created Fixed Commission Amount.
        /// </summary>
        ReactiveCommand<Unit, Unit> Save { get; }

        /// <summary>
        /// Gets the command to cancel and go back to viewing existing Fixed Commission Amounts.
        /// </summary>
        ReactiveCommand<Unit, Unit> Cancel { get; }

        /// <summary>
        /// Gets the collection of all available products for selection.
        /// </summary>
        ReadOnlyObservableCollection<Product> Products { get; }

        /// <summary>
        /// Gets or sets the selected product to apply this Fixed Commission Amount to.
        /// </summary>
        Product SelectedProduct { get; set; }

        /// <summary>
        /// Gets or sets the Fixed Commission Amount to apply to the selected product.
        /// </summary>
        decimal CommissionAmount { get; set; }

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
