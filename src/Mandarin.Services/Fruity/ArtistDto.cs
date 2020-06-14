using System;
using Newtonsoft.Json;

namespace Mandarin.Services.Fruity
{
    /// <summary>
    /// Represents the Artist as a JSON object received by Fruity.
    /// </summary>
    [JsonObject]
    internal sealed class ArtistDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistDto"/> class.
        /// </summary>
        /// <param name="stockistCode">The artist's unique stockist code.</param>
        /// <param name="stockistName">The artist's name.</param>
        /// <param name="status">The current status of the artist for commissions.</param>
        /// <param name="description">The artist's personal description.</param>
        /// <param name="rate">The artist's commission rate.</param>
        /// <param name="startDate">The artist's commission start date.</param>
        /// <param name="endDate">The artist's commission end date.</param>
        /// <param name="imageUrl">The artist's personal banner image.</param>
        /// <param name="emailAddress">The artist's email address.</param>
        /// <param name="twitterHandle">The artist's Twitter handle.</param>
        /// <param name="instagramHandle">The artist's Instagram handle.</param>
        /// <param name="facebookHandle">The artist's Facebook handle.</param>
        /// <param name="tumblrHandle">The artist's Tumblr handle.</param>
        /// <param name="websiteUrl">The url to the artist's personal website.</param>
        public ArtistDto(string stockistCode,
                         string stockistName,
                         string status,
                         string description,
                         int rate,
                         DateTime? startDate,
                         DateTime? endDate,
                         string imageUrl,
                         string emailAddress,
                         string twitterHandle,
                         string instagramHandle,
                         string facebookHandle,
                         string tumblrHandle,
                         string websiteUrl)
        {
            this.StockistCode = stockistCode;
            this.StockistName = stockistName;
            this.Status = status;
            this.Description = description;
            this.Rate = rate;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.ImageUrl = imageUrl;
            this.EmailAddress = emailAddress;
            this.TwitterHandle = twitterHandle;
            this.InstagramHandle = instagramHandle;
            this.FacebookHandle = facebookHandle;
            this.TumblrHandle = tumblrHandle;
            this.WebsiteUrl = websiteUrl;
        }

        /// <summary>
        /// Gets the artist's unique stockist code.
        /// </summary>
        [JsonProperty("stockist_code")]
        public string StockistCode { get; }

        /// <summary>
        /// Gets the artist's name.
        /// </summary>
        [JsonProperty("stockist_name")]
        public string StockistName { get; }

        /// <summary>
        /// Gets the artist's current status for commissions.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; }

        /// <summary>
        /// Gets the artist's personal description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; }

        /// <summary>
        /// Gets the artist's commission rate.
        /// </summary>
        [JsonProperty("rate")]
        public int Rate { get; }

        /// <summary>
        /// Gets the artist's commission start date.
        /// </summary>
        [JsonProperty("start_date")]
        public DateTime? StartDate { get; }

        /// <summary>
        /// Gets the artist's commission end date.
        /// </summary>
        [JsonProperty("end_date")]
        public DateTime? EndDate { get; }

        /// <summary>
        /// Gets the artist's personal banner image.
        /// </summary>
        [JsonProperty("image_url")]
        public string ImageUrl { get; }

        /// <summary>
        /// Gets the artist's email address.
        /// </summary>
        [JsonProperty("email_address")]
        public string EmailAddress { get; }

        /// <summary>
        /// Gets the artist's Twitter handle.
        /// </summary>
        [JsonProperty("twitter_handle")]
        public string TwitterHandle { get; }

        /// <summary>
        /// Gets the artist's Instagram handle.
        /// </summary>
        [JsonProperty("instagram_handle")]
        public string InstagramHandle { get; }

        /// <summary>
        /// Gets the artist's Facebook handle.
        /// </summary>
        [JsonProperty("facebook_handle")]
        public string FacebookHandle { get; }

        /// <summary>
        /// Gets the artist's Tumblr handle.
        /// </summary>
        [JsonProperty("tumblr_handle")]
        public string TumblrHandle { get; }

        /// <summary>
        /// Gets the url to the artist's personal website.
        /// </summary>
        [JsonProperty("website_url")]
        public string WebsiteUrl { get; }
    }
}
