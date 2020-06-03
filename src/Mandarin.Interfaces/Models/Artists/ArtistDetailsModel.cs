using System;

namespace Mandarin.Models.Artists
{
    // TODO: Move the URL formatting from the mapper to this class.
    public class ArtistDetailsModel
    {
        public ArtistDetailsModel(string stockistCode,
                                  string name,
                                  string description,
                                  decimal rate,
                                  string emailAddress,
                                  Uri imageUrl,
                                  Uri twitterUrl,
                                  Uri instagramUrl,
                                  Uri facebookUrl,
                                  Uri tumblrUrl,
                                  Uri websiteUrl)
        {
            this.StockistCode = stockistCode;
            this.Name = name;
            this.Description = description;
            this.Rate = rate;
            this.EmailAddress = emailAddress;
            this.ImageUrl = imageUrl;
            this.TwitterUrl = twitterUrl;
            this.InstagramUrl = instagramUrl;
            this.FacebookUrl = facebookUrl;
            this.TumblrUrl = tumblrUrl;
            this.WebsiteUrl = websiteUrl;
        }

        public string StockistCode { get; }
        public string Name { get; }
        public string Description { get; }
        public decimal Rate { get; }
        public string EmailAddress { get; }
        public Uri ImageUrl { get; }
        public Uri TwitterUrl { get; }
        public Uri InstagramUrl { get; }
        public Uri FacebookUrl { get; }
        public Uri TumblrUrl { get; }
        public Uri WebsiteUrl { get; }

        public override string ToString()
        {
            return $"{this.StockistCode}: {this.Name}";
        }
    }
}
