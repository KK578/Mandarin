using System;
using Mandarin.Models.Artists;

namespace Mandarin.ViewModels.Artists
{
    /// <inheritdoc />
    internal sealed class ArtistViewModel : IArtistViewModel
    {
        private readonly ArtistDetailsModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistViewModel"/> class.
        /// </summary>
        /// <param name="model">The underlying artist detail model.</param>
        public ArtistViewModel(ArtistDetailsModel model)
        {
            this.model = model;
        }

        /// <inheritdoc/>
        public string Name => this.model.Name;

        /// <inheritdoc/>
        public string Description => this.model.Description;

        /// <inheritdoc/>
        public Uri ImageUrl => this.model.ImageUrl;

        /// <inheritdoc/>
        public Uri WebsiteUrl => this.model.WebsiteUrl;

        /// <inheritdoc/>
        public Uri TwitterUrl => this.model.TwitterUrl;

        /// <inheritdoc/>
        public Uri InstagramUrl => this.model.InstagramUrl;

        /// <inheritdoc/>
        public Uri FacebookUrl => this.model.FacebookUrl;

        /// <inheritdoc/>
        public Uri TumblrUrl => this.model.TumblrUrl;
    }
}
