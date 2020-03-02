namespace Mandarin.ViewModels.Index.Carousel
{
    internal sealed class CarouselImageViewModel : ICarouselImageViewModel
    {
        public CarouselImageViewModel(string sourceUrl, string description)
        {
            SourceUrl = sourceUrl;
            Description = description;
        }

        public string SourceUrl { get; }
        public string Description { get; }
    }
}
