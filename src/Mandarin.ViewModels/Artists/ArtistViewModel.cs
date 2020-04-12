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
        public Uri ImageUrl => ArtistViewModel.CreateUri(this.model.Image);
        public Uri WebsiteUrl => ArtistViewModel.CreateUri(this.model.Website);
        public Uri TwitterUrl => ArtistViewModel.CreateUri(this.model.Twitter, "https://twitter.com/{0}");
        public Uri InstagramUrl => ArtistViewModel.CreateUri(this.model.Instagram, "https://instagram.com/{0}");
        public Uri FacebookUrl => ArtistViewModel.CreateUri(this.model.Facebook, "https://facebook.com/{0}");
        public Uri TumblrUrl => ArtistViewModel.CreateUri(this.model.Tumblr, "https://{0}.tumblr.com/");

        private static Uri CreateUri(string uri, string format = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return null;
            }

            return string.IsNullOrEmpty(format) ? new Uri(uri) : new Uri(string.Format(format, uri));
        }
    }
}
