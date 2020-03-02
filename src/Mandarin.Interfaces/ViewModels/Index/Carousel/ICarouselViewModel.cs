using System.Collections.Generic;

namespace Mandarin.ViewModels.Index.Carousel
{
    public interface ICarouselViewModel
    {
        IReadOnlyList<ICarouselImageViewModel> Images { get; }
    }
}
