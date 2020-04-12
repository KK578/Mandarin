using System.ComponentModel.DataAnnotations;

namespace Mandarin.Models.Artists
{
    public class ArtistDetailsModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [Url] public string ImageUri { get; set; }
        [Url] public string TwitterUri { get; set; }
        [Url] public string InstagramUri { get; set; }
        [Url] public string FacebookUri { get; set; }
        [Url] public string TumblrUri { get; set; }
        [Url] public string WebsiteUri { get; set; }
    }
}
