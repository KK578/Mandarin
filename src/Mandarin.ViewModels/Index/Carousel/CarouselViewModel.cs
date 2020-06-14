using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.Index.Carousel
{
    /// <inheritdoc />
    internal sealed class CarouselViewModel : ICarouselViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselViewModel"/> class.
        /// </summary>
        public CarouselViewModel()
        {
            this.Images = new List<IMandarinImageViewModel>
            {
                new MandarinImageViewModel("/static/images/about/ShopFront.jpg", "The Little Mandarin - Shop Front"),
                new MandarinImageViewModel("/static/images/about/ShopWallDisplay.jpg", "The Little Mandarin - Display"),
                new MandarinImageViewModel("/static/images/about/ShopDisplay.jpg", "The Little Mandarin - Interior"),
                new MandarinImageViewModel("/static/images/about/Macarons.jpg", "The Mini Mandarin - Macarons"),
            }.AsReadOnly();
        }

        /// <inheritdoc/>
        public IReadOnlyList<IMandarinImageViewModel> Images { get; }
    }
}
