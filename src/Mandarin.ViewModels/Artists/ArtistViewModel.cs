using System;

namespace Mandarin.ViewModels.Artists
{
    internal sealed class ArtistViewModel : IArtistViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Uri ImageUrl { get; set; }
        public Uri WebsiteUrl { get; set; }
        public Uri TwitterUrl { get; set; }
        public Uri InstagramUrl { get; set; }
        public Uri FacebookUrl { get; set; }
        public Uri TumblrUrl { get; set; }
    }
}
