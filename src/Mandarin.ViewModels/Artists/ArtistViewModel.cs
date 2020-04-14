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
        public Uri ImageUrl => this.model.ImageUrl;
        public Uri WebsiteUrl => this.model.WebsiteUrl;
        public Uri TwitterUrl => this.model.TwitterUrl;
        public Uri InstagramUrl => this.model.InstagramUrl;
        public Uri FacebookUrl => this.model.FacebookUrl;
        public Uri TumblrUrl => this.model.TumblrUrl;
    }
}
