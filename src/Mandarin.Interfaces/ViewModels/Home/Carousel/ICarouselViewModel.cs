using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.Home.Carousel
{
    /// <summary>
    /// Represents the component content for The Little Mandarin's image carousel.
    /// </summary>
    public interface ICarouselViewModel
    {
        /// <summary>
        /// Gets the list of image details to be included in the carousel.
        /// </summary>
        IReadOnlyList<IMandarinImageViewModel> Images { get; }
    }
}
