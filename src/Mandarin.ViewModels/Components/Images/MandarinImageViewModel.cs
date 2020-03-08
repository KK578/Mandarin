using System;

namespace Mandarin.ViewModels.Components.Images
{
    internal sealed class MandarinImageViewModel : IMandarinImageViewModel
    {
        public MandarinImageViewModel(string sourceUrl, string description)
        {
            this.SourceUrl = ParseUri(sourceUrl) ?? throw new ArgumentException($"Invalid sourceUrl {sourceUrl}", nameof(sourceUrl));
            this.Description = description;
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
