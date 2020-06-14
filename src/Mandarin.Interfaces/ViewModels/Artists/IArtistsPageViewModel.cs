using System.Collections.Generic;

namespace Mandarin.ViewModels.Artists
{
    /// <summary>
    /// Represents the page content for the Artists page.
    /// </summary>
    public interface IArtistsPageViewModel : IViewModel
    {
        /// <summary>
        /// Gets a value indicating whether the list of artists is still loading.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Gets the list of all artists that should be displayed.
        /// </summary>
        IReadOnlyList<IArtistViewModel> ViewModels { get; }
    }
}
