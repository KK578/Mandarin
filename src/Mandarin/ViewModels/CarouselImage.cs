namespace Mandarin.ViewModels
{
    public class CarouselImage
    {
        public CarouselImage(string sourceUrl, string description)
        {
            SourceUrl = sourceUrl;
            Description = description;
        }

        public string SourceUrl { get; }
        public string Description { get; }
    }
}
