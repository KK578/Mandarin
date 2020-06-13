using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.Index
{
    /// <summary>
    /// Represents the page content for The Little Mandarin's home page.
    /// </summary>
    public interface IIndexPageViewModel
    {
        /// <summary>
        /// Gets the main heading text content.
        /// </summary>
        IReadOnlyList<string> Paragraphs { get; }

        /// <summary>
        /// Gets the image details for the eGiftCard animation.
        /// </summary>
        IMandarinImageViewModel GiftCardImageViewModel { get; }
    }
}
