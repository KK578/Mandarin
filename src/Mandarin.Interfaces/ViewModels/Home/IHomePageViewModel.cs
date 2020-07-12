using Mandarin.ViewModels.Home.Carousel;
using Mandarin.ViewModels.Home.MandarinMap;
using Mandarin.ViewModels.Home.OpeningTimes;

namespace Mandarin.ViewModels.Home
{
    /// <summary>
    /// Represents the page content for The Little Mandarin's home page.
    /// </summary>
    public interface IHomePageViewModel
    {
        /// <summary>
        /// Gets the view model for the carousel component.
        /// </summary>
        ICarouselViewModel CarouselViewModel { get; }

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
