using System;

namespace Mandarin.ViewModels.Components.Images
{
    /// <inheritdoc />
    internal sealed class MandarinImageViewModel : IMandarinImageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinImageViewModel"/> class.
        /// </summary>
        /// <param name="sourceUrl">The image's url.</param>
        /// <param name="description">The image's alt description.</param>
        public MandarinImageViewModel(string sourceUrl, string description)
        {
            this.SourceUrl = MandarinImageViewModel.ParseUri(sourceUrl) ??
                             throw new ArgumentException($"Invalid sourceUrl {sourceUrl}", nameof(sourceUrl));
            this.Description = description;
        }

        /// <inheritdoc/>
        public Uri SourceUrl { get; }

        /// <inheritdoc/>
        public string Description { get; }

        private static Uri ParseUri(string sourceUrl)
        {
            if (Uri.IsWellFormedUriString(sourceUrl, UriKind.Absolute))
            {
                return new Uri(sourceUrl);
            }

            return Uri.IsWellFormedUriString(sourceUrl, UriKind.Relative) ? new Uri(sourceUrl, UriKind.Relative) : null;
        }
    }
}
