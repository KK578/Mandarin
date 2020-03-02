using System.Collections.Generic;

namespace Mandarin.ViewModels
{
    internal sealed class CarouselViewModel : ICarouselViewModel
    {
        public IReadOnlyList<CarouselImage> Images { get; }

        public CarouselViewModel()
        {
            Images = new List<CarouselImage>
            {
                new CarouselImage("https://thelittlemandarin.co.uk/static/images/about/TheLittleMandarin-1.jpg", "The Little Mandarin - Shop Front"),
                new CarouselImage("https://thelittlemandarin.co.uk/static/images/about/TheLittleMandarin-2.jpg", "The Little Mandarin - Display"),
                new CarouselImage("https://thelittlemandarin.co.uk/static/images/about/TheLittleMandarin-3.jpg", "The Little Mandarin - Interior"),
                new CarouselImage("https://thelittlemandarin.co.uk/static/images/about/TheLittleMandarin-4.jpg", "The Mini Mandarin - Macarons"),
            }.AsReadOnly();
        }
    }
}
