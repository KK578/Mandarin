using System;

namespace Mandarin.ViewModels.Index.Carousel
{
    internal sealed class CarouselImageViewModel : ICarouselImageViewModel
    {
        public CarouselImageViewModel(string sourceUrl, string description)
        {
            SourceUrl = ParseUri(sourceUrl) ?? throw new ArgumentException($"Invalid sourceUrl {sourceUrl}", nameof(sourceUrl));
            Description = description;
        }

        public Uri SourceUrl { get; }
        public string Description { get; }

        private static Uri ParseUri(string sourceUrl)
        {
            if (Uri.IsWellFormedUriString(sourceUrl, UriKind.Absolute))
                return new Uri(sourceUrl);

            return Uri.IsWellFormedUriString(sourceUrl, UriKind.Relative) ? new Uri(sourceUrl, UriKind.Relative) : null;
        }
    }
}
