using Mandarin.ViewModels.Home.Carousel;
using Mandarin.ViewModels.Home.MandarinMap;
using Mandarin.ViewModels.Home.OpeningTimes;

namespace Mandarin.ViewModels.Home
{
    /// <inheritdoc />
    internal sealed class HomePageViewModel : IHomePageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomePageViewModel"/> class.
        /// </summary>
        /// <param name="pageContentModel">The public website content model.</param>
        public HomePageViewModel(PageContentModel pageContentModel)
        {
            this.CarouselViewModel = new CarouselViewModel(pageContentModel);
            this.MapViewModel = new MandarinMapViewModel(pageContentModel);
            this.OpeningTimesViewModel = new OpeningTimesViewModel(pageContentModel);
        }

        /// <inheritdoc/>
        public ICarouselViewModel CarouselViewModel { get; }

        /// <inheritdoc/>
        public IMandarinMapViewModel MapViewModel { get; }

        /// <inheritdoc/>
        public IOpeningTimesViewModel OpeningTimesViewModel { get; }
    }
}
