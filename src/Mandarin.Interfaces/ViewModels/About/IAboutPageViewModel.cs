using Mandarin.ViewModels.Components.Images;
using Microsoft.AspNetCore.Components;

namespace Mandarin.ViewModels.About
{
    /// <summary>
    /// Represents the page content for The Little Mandarin's home page.
    /// </summary>
    public interface IAboutPageViewModel
    {
        /// <summary>
        /// Gets the view model for the image component.
        /// </summary>
        IMandarinImageViewModel ImageViewModel { get; }

        /// <summary>
        /// Gets the main section's text content.
        /// </summary>
        MarkupString MainContent { get; }

        /// <summary>
        /// Gets the gift card section's text content.
        /// </summary>
        MarkupString GiftCardContent { get; }

        /// <summary>
        /// Gets the image details for the eGiftCard animation.
        /// </summary>
        IMandarinImageViewModel GiftCardImageViewModel { get; }
    }
}
