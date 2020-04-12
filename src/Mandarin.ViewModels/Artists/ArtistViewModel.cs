using System;
using Mandarin.Models.Artists;

namespace Mandarin.ViewModels.Artists
{
    internal sealed class ArtistViewModel : IArtistViewModel
    {
        private readonly ArtistDetailsModel model;

        public ArtistViewModel(ArtistDetailsModel model)
        {
            this.model = model;
        }

        public string Name => this.model.Name;
        public string Description => this.model.Description;
        public Uri ImageUrl => new Uri(this.model.ImageUri);
        public Uri WebsiteUrl => new Uri(this.model.WebsiteUri);
        public Uri TwitterUrl => new Uri(this.model.TwitterUri);
        public Uri InstagramUrl => new Uri(this.model.InstagramUri);
        public Uri FacebookUrl => new Uri(this.model.FacebookUri);
        public Uri TumblrUrl => new Uri(this.model.TumblrUri);
    }
}
