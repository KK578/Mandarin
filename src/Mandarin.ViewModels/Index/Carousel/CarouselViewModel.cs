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
                new CarouselImageViewModel("https://thelittlemandarin.co.uk/static/images/about/TheLittleMandarin-1.jpg", "The Little Mandarin - Shop Front"),
                new CarouselImageViewModel("https://thelittlemandarin.co.uk/static/images/about/TheLittleMandarin-2.jpg", "The Little Mandarin - Display"),
                new CarouselImageViewModel("https://thelittlemandarin.co.uk/static/images/about/TheLittleMandarin-3.jpg", "The Little Mandarin - Interior"),
                new CarouselImageViewModel("https://thelittlemandarin.co.uk/static/images/about/TheLittleMandarin-4.jpg", "The Mini Mandarin - Macarons"),
            }.AsReadOnly();
        }
    }
}
