using System.Collections.Generic;
using System.Linq;
using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.Index.Carousel
{
    /// <inheritdoc />
    internal sealed class CarouselViewModel : ICarouselViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselViewModel"/> class.
        /// </summary>
        /// <param name="pageContentModel">The public website content model.</param>
        public CarouselViewModel(PageContentModel pageContentModel)
        {
            this.Images = pageContentModel.GetAll<ImageUrlModel>("About", "Carousel").Select(x => new MandarinImageViewModel(x)).ToList().AsReadOnly();
        }

        /// <inheritdoc/>
        public IReadOnlyList<IMandarinImageViewModel> Images { get; }
    }
}
