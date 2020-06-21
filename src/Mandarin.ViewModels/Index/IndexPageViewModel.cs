using Mandarin.ViewModels.Components.Images;
using Mandarin.ViewModels.Index.Carousel;
using Mandarin.ViewModels.Index.MandarinMap;
using Mandarin.ViewModels.Index.OpeningTimes;
using Microsoft.AspNetCore.Components;

namespace Mandarin.ViewModels.Index
{
    /// <inheritdoc />
    internal sealed class IndexPageViewModel : IIndexPageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexPageViewModel"/> class.
        /// </summary>
        /// <param name="pageContentModel">The public website content model.</param>
        public IndexPageViewModel(PageContentModel pageContentModel)
        {
            this.CarouselViewModel = new CarouselViewModel(pageContentModel);
            this.MainContent = pageContentModel.GetMarkupString("About", "MainText");
            this.GiftCardContent = pageContentModel.GetMarkupString("About", "GiftCards", "Text");
            this.GiftCardImageViewModel = new MandarinImageViewModel(pageContentModel.Get<ImageUrlModel>("About", "GiftCards", "AnimationImage"));
            this.MapViewModel = new MandarinMapViewModel(pageContentModel);
            this.OpeningTimesViewModel = new OpeningTimesViewModel(pageContentModel);
        }

        /// <inheritdoc/>
        public ICarouselViewModel CarouselViewModel { get; }

        /// <inheritdoc/>
        public MarkupString MainContent { get; }

        /// <inheritdoc/>
        public MarkupString GiftCardContent { get; }

        /// <inheritdoc/>
        public IMandarinImageViewModel GiftCardImageViewModel { get; }

        /// <inheritdoc/>
        public IMandarinMapViewModel MapViewModel { get; }

        /// <inheritdoc/>
        public IOpeningTimesViewModel OpeningTimesViewModel { get; }
    }
}
