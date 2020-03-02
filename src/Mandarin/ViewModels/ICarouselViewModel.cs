using System.Collections.Generic;

namespace Mandarin.ViewModels
{
    public interface ICarouselViewModel
    {
        IReadOnlyList<CarouselImage> Images { get; }
    }
}
