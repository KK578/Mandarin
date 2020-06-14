using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.MiniMandarin
{
    /// <summary>
    /// Represents the page content for The Mini Mandarin page.
    /// </summary>
    public interface IMiniMandarinPageViewModel
    {
        /// <summary>
        /// Gets the main heading text content.
        /// </summary>
        IReadOnlyList<string> Paragraphs { get; }

        /// <summary>
        /// Gets the image details for the heading banner.
        /// </summary>
        IMandarinImageViewModel BannerImageViewModel { get; }

        /// <summary>
        /// Gets the list of image details for the listed macaron flavours to be displayed.
        /// </summary>
        IReadOnlyList<IMandarinImageViewModel> MacaronImageViewModels { get; }
    }
}
