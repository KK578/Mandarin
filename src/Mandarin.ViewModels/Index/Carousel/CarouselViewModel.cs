using System.Collections.Generic;

namespace Mandarin.ViewModels.Index.Carousel
{
    internal sealed class CarouselViewModel : ICarouselViewModel
    {
        public IReadOnlyList<ICarouselImageViewModel> Images { get; }

        public CarouselViewModel()
        {
            Images = new List<ICarouselImageViewModel>
            {
                new CarouselImageViewModel("/images/about/ShopFront.jpg", "The Little Mandarin - Shop Front"),
                new CarouselImageViewModel("/images/about/ShopWallDisplay.jpg", "The Little Mandarin - Display"),
                new CarouselImageViewModel("/images/about/ShopDisplay.jpg", "The Little Mandarin - Interior"),
                new CarouselImageViewModel("/images/about/Macarons.jpg", "The Mini Mandarin - Macarons"),
            }.AsReadOnly();
        }
    }
}
