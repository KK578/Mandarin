using System.Collections.Generic;
using Mandarin.ViewModels.Index.Carousel;

namespace Mandarin.ViewModels.MiniMandarin
{
    public interface IMiniMandarinPageViewModel
    {
        IReadOnlyList<string> Paragraphs { get; }
        ICarouselImageViewModel BannerImageViewModel { get; }
        IReadOnlyList<ICarouselImageViewModel> MacaronImageViewModels { get; }
    }
}
