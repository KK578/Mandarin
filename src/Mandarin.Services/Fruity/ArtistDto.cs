using Newtonsoft.Json;

namespace Mandarin.Services.Fruity
{
    [JsonObject]
    internal sealed class ArtistDto
    {
        [JsonProperty("stockist_name")] public string StockistName { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("image_url")] public string ImageUrl { get; set; }
        [JsonProperty("twitter_handle")] public string TwitterHandle { get; set; }
        [JsonProperty("instagram_handle")] public string InstagramHandle { get; set; }
        [JsonProperty("facebook_handle")] public string FacebookHandle { get; set; }
        [JsonProperty("tumblr_handle")] public string TumblrHandle { get; set; }
        [JsonProperty("website_url")] public string WebsiteUrl { get; set; }
    }
}
