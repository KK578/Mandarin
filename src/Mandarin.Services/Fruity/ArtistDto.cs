using System;
using Newtonsoft.Json;

namespace Mandarin.Services.Fruity
{
    [JsonObject]
    internal sealed class ArtistDto
    {
        public ArtistDto(string stockistCode, string stockistName, string status, string description, int rate, DateTime? startDate, DateTime? endDate, string imageUrl, string emailAddress, string twitterHandle, string instagramHandle, string facebookHandle, string tumblrHandle, string websiteUrl)
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

        [JsonProperty("stockist_code")] public string StockistCode { get; }
        [JsonProperty("stockist_name")] public string StockistName { get; }
        [JsonProperty("status")] public string Status { get; }
        [JsonProperty("description")] public string Description { get; }
        [JsonProperty("rate")] public int Rate { get; }
        [JsonProperty("start_date")] public DateTime? StartDate { get; }
        [JsonProperty("end_date")] public DateTime? EndDate { get; }
        [JsonProperty("image_url")] public string ImageUrl { get; }
        [JsonProperty("email_address")] public string EmailAddress { get; }
        [JsonProperty("twitter_handle")] public string TwitterHandle { get; }
        [JsonProperty("instagram_handle")] public string InstagramHandle { get; }
        [JsonProperty("facebook_handle")] public string FacebookHandle { get; }
        [JsonProperty("tumblr_handle")] public string TumblrHandle { get; }
        [JsonProperty("website_url")] public string WebsiteUrl { get; }
    }
}
