using System;
using Mandarin.Models.Artists;

namespace Mandarin.ViewModels.Artists
{
    /// <inheritdoc />
    internal sealed class ArtistViewModel : IArtistViewModel
    {
        private const string FacebookFormat = "https://facebook.com/{0}";
        private const string TwitterFormat = "https://twitter.com/{0}";
        private const string InstagramFormat = "https://instagram.com/{0}";
        private const string TumblrFormat = "https://{0}.tumblr.com/";

        private readonly Stockist model;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistViewModel"/> class.
        /// </summary>
        /// <param name="model">The underlying artist detail model.</param>
        public ArtistViewModel(Stockist model)
        {
            this.model = model;
        }

        /// <inheritdoc/>
        public string ShortDisplayName => this.model.Details.ShortDisplayName;

        /// <inheritdoc/>
        public string FullDisplayName => this.model.Details.FullDisplayName;

        /// <inheritdoc/>
        public string Description => this.model.Details.Description;

        /// <inheritdoc/>
        public Uri GridImageUrl => ArtistViewModel.ParseUri(this.model.Details.ThumbnailImageUrl);

        /// <inheritdoc/>
        public Uri BannerImageUrl => ArtistViewModel.ParseUri(this.model.Details.BannerImageUrl);

        /// <inheritdoc/>
        public Uri WebsiteUrl => ArtistViewModel.ParseUri(this.model.Details.WebsiteUrl);

        /// <inheritdoc/>
        public Uri TwitterUrl => ArtistViewModel.ParseUri(this.model.Details.TwitterHandle, ArtistViewModel.TwitterFormat);

        /// <inheritdoc/>
        public Uri InstagramUrl => ArtistViewModel.ParseUri(this.model.Details.InstagramHandle, ArtistViewModel.InstagramFormat);

        /// <inheritdoc/>
        public Uri FacebookUrl => ArtistViewModel.ParseUri(this.model.Details.FacebookHandle, ArtistViewModel.FacebookFormat);

        /// <inheritdoc/>
        public Uri TumblrUrl => ArtistViewModel.ParseUri(this.model.Details.TumblrHandle, ArtistViewModel.TumblrFormat);

        private static Uri ParseUri(string uri, string format = "{0}")
        {
            return string.IsNullOrEmpty(uri) ? null : new Uri(string.Format(format, uri));
        }
    }
}
