using System;

namespace Mandarin.Models.Artists
{
    public class ArtistDetailsModel
    {
        public ArtistDetailsModel(string name, string description, Uri imageUrl, Uri twitterUrl, Uri instagramUrl, Uri facebookUrl, Uri tumblrUrl, Uri websiteUrl)
        {
            this.Name = name;
            this.Description = description;
            this.ImageUrl = imageUrl;
            this.TwitterUrl = twitterUrl;
            this.InstagramUrl = instagramUrl;
            this.FacebookUrl = facebookUrl;
            this.TumblrUrl = tumblrUrl;
            this.WebsiteUrl = websiteUrl;
        }

        public string Name { get; }
        public string Description { get; }
        public Uri ImageUrl { get; }
        public Uri TwitterUrl { get; }
        public Uri InstagramUrl { get; }
        public Uri FacebookUrl { get; }
        public Uri TumblrUrl { get; }
        public Uri WebsiteUrl { get; }
    }
}
