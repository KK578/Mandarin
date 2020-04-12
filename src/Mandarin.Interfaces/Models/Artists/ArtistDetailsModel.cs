using System.ComponentModel.DataAnnotations;

namespace Mandarin.Models.Artists
{
    public class ArtistDetailsModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [Url] public string Image { get; set; }
        [Url] public string Twitter { get; set; }
        [Url] public string Instagram { get; set; }
        [Url] public string Facebook { get; set; }
        [Url] public string Tumblr { get; set; }
        [Url] public string Website { get; set; }
    }
}
