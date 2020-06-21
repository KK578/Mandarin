using Mandarin.ViewModels.Components.Images;
using Mandarin.ViewModels.Index.Carousel;
using Mandarin.ViewModels.Index.MandarinMap;
using Mandarin.ViewModels.Index.OpeningTimes;
using Microsoft.AspNetCore.Components;

namespace Mandarin.ViewModels.Index
{
    /// <summary>
    /// Represents the page content for The Little Mandarin's home page.
    /// </summary>
    public interface IIndexPageViewModel
    {
        /// <summary>
        /// Gets the view model for the carousel component.
        /// </summary>
        ICarouselViewModel CarouselViewModel { get; }

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

        /// <summary>
        /// Gets the view model for the embedded map component.
        /// </summary>
        IMandarinMapViewModel MapViewModel { get; }

        /// <summary>
        /// Gets the view model for the opening times component.
        /// </summary>
        IOpeningTimesViewModel OpeningTimesViewModel { get; }
    }
}
