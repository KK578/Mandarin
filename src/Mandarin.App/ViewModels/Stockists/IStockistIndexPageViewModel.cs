using System.Collections.Generic;
using Mandarin.Models.Artists;
using Mandarin.MVVM.Commands;
using Mandarin.MVVM.ViewModels;

namespace Mandarin.App.ViewModels.Stockists
{
    /// <summary>
    /// Represents the <see cref="IViewModel"/> for the Stockist Index page.
    /// </summary>
    public interface IStockistIndexPageViewModel : IViewModel
    {
        /// <summary>
        /// Gets the list of all currently available stockists.
        /// </summary>
        IReadOnlyList<Stockist> Stockists { get; }

        /// <summary>
        /// Gets or sets the selected stockist.
        /// </summary>
        Stockist SelectedStockist { get; set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Stockists"/> data is currently being loaded.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Gets a command to direct the user to create a new <see cref="Stockist"/>.
        /// </summary>
        ICommand CreateNewStockistCommand { get; }

        /// <summary>
        /// Gets a command to direct the user to edit the selected <see cref="Stockist"/>.
        /// </summary>
        ICommand EditStockistCommand { get; }
    }
}
