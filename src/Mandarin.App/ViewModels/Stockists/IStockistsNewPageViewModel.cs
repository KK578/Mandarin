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
    public interface IStockistsNewPageViewModel : IViewModel
    {
        /// <summary>
        /// Gets the selected stockist.
        /// </summary>
        Stockist Stockist { get; }

        /// <summary>
        /// Gets the list of available <see cref="CommissionRateGroup"/> options.
        /// </summary>
        IReadOnlyList<CommissionRateGroup> CommissionRateGroups { get; }

        /// <summary>
        /// Gets the command to activate on closing the page.
        /// </summary>
        ICommand CloseCommand { get; }

        /// <summary>
        /// Gets the command to activate on submitting the page.
        /// </summary>
        ICommand SubmitCommand { get; }
    }
}
