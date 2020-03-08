using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.Index.Carousel
{
    public interface ICarouselViewModel
    {
        IReadOnlyList<IMandarinImageViewModel> Images { get; }
    }
}
