using System;

namespace Mandarin.ViewModels.Index.Carousel
{
    internal sealed class CarouselImageViewModel : ICarouselImageViewModel
    {
        public CarouselImageViewModel(string sourceUrl, string description)
        {
            SourceUrl = Uri.IsWellFormedUriString(sourceUrl, UriKind.Absolute)
                ? new Uri(sourceUrl)
                : Uri.IsWellFormedUriString(sourceUrl, UriKind.Relative)
                    ? new Uri(sourceUrl, UriKind.Relative)
                    : throw new ArgumentException($"Invalid sourceUrl {sourceUrl}", nameof(sourceUrl));
            Description = description;
        }

        public Uri SourceUrl { get; }
        public string Description { get; }
    }
}
