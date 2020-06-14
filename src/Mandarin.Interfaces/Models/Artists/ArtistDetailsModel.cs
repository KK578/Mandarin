using System;

namespace Mandarin.Models.Artists
{
    /// <summary>
    /// Artist's personal detail model for display on public website.
    /// </summary>
    /// TODO: Move the URL formatting from the mapper to this class.
    /// TODO: Split commission details to a different class.
    public class ArtistDetailsModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistDetailsModel"/> class.
        /// </summary>
        /// <param name="stockistCode">Artist's unique stockist code.</param>
        /// <param name="name">Artist's full name.</param>
        /// <param name="description">Artist's public tagline description.</param>
        /// <param name="rate">Artist's Commission rate as a percentage.</param>
        /// <param name="emailAddress">Artist's email address for commission records.</param>
        /// <param name="imageUrl">Artist's public image url.</param>
        /// <param name="twitterUrl">Artist's public Twitter url.</param>
        /// <param name="instagramUrl">Artists' public Instagram url.</param>
        /// <param name="facebookUrl">Artist's public Facebook url.</param>
        /// <param name="tumblrUrl">Artist's public Tumblr url.</param>
        /// <param name="websiteUrl">Artist's public personal website url.</param>
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

        /// <summary>
        /// Gets the artist's unique stockist code.
        /// </summary>
        public string StockistCode { get; }

        /// <summary>
        /// Gets the artist's full name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the artist's biography.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the artist's commission rate.
        /// </summary>
        public decimal Rate { get; }

        /// <summary>
        /// Gets the artist's email address.
        /// </summary>
        public string EmailAddress { get; }

        /// <summary>
        /// Gets the artist's public image banner.
        /// </summary>
        public Uri ImageUrl { get; }

        /// <summary>
        /// Gets the artist's Twitter url.
        /// </summary>
        public Uri TwitterUrl { get; }

        /// <summary>
        /// Gets the artist's Instagram url.
        /// </summary>
        public Uri InstagramUrl { get; }

        /// <summary>
        /// Gets the artist's Facebook url.
        /// </summary>
        public Uri FacebookUrl { get; }

        /// <summary>
        /// Gets the artist's Tumblr url.
        /// </summary>
        public Uri TumblrUrl { get; }

        /// <summary>
        /// Gets the artist's personal website url.
        /// </summary>
        public Uri WebsiteUrl { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.StockistCode}: {this.Name}";
        }
    }
}
