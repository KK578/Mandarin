using System;

namespace Mandarin.ViewModels.Artists
{
    public interface IArtistViewModel
    {
        string Name { get; }
        string Description { get; }
        Uri ImageUrl { get; }
        Uri WebsiteUrl { get; }
        Uri TwitterUrl { get; }
        Uri InstagramUrl { get; }
        Uri FacebookUrl { get; }
        Uri TumblrUrl { get; }
    }
}
