using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;
using Microsoft.AspNetCore.Components;

namespace Mandarin.ViewModels.Macarons
{
    /// <summary>
    /// Represents the page content for the Macarons page.
    /// </summary>
    public interface IMacaronsPageViewModel
    {
        /// <summary>
        /// Gets the main section's text content.
        /// </summary>
        MarkupString TextContent { get; }

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
