using System.Collections.Generic;
using Mandarin.Models.Artists;
using Mandarin.Models.Commissions;
using Mandarin.MVVM.Commands;
using Mandarin.MVVM.ViewModels;

namespace Mandarin.App.ViewModels.Stockists
{
    /// <summary>
    /// Represents the <see cref="IViewModel"/> for the "New Stockist" page.
    /// </summary>
    public interface IStockistsEditPageViewModel : IViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel allows editing.
        /// </summary>
        bool IsEditing { get; set; }

        /// <summary>
        /// Gets or sets the selected stockist's code.
        /// </summary>
        string StockistCode { get; set; }

        /// <summary>
        /// Gets or sets the selected stockist.
        /// </summary>
        Stockist Stockist { get; set; }

        /// <summary>
        /// Gets the list of available <see cref="CommissionRateGroup"/> options.
        /// </summary>
        IReadOnlyList<CommissionRateGroup> CommissionRateGroups { get; }

        /// <summary>
        /// Gets the command to activate on closing the page.
        /// </summary>
        ICommand CloseCommand { get; }
    }
}
