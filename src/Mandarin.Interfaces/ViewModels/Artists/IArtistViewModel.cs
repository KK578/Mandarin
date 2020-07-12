using System;

namespace Mandarin.ViewModels.Artists
{
    /// <summary>
    /// Represents the component content for an artist.
    /// </summary>
    public interface IArtistViewModel
    {
        /// <summary>
        /// Gets the artist's thumbnail display name.
        /// </summary>
        string ShortDisplayName { get; }

        /// <summary>
        /// Gets the artist's banner display name.
        /// </summary>
        string FullDisplayName { get; }

        /// <summary>
        /// Gets the artist's biography.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the artist's public image banner.
        /// </summary>
        Uri GridImageUrl { get; }

        /// <summary>
        /// Gets the artist's public image banner.
        /// </summary>
        Uri BannerImageUrl { get; }

        /// <summary>
        /// Gets the artist's personal website url.
        /// </summary>
        Uri WebsiteUrl { get; }

        /// <summary>
        /// Gets the artist's Twitter url.
        /// </summary>
        Uri TwitterUrl { get; }

        /// <summary>
        /// Gets the artist's Instagram url.
        /// </summary>
        Uri InstagramUrl { get; }

        /// <summary>
        /// Gets the artist's Facebook url.
        /// </summary>
        Uri FacebookUrl { get; }

        /// <summary>
        /// Gets the artist's Tumblr url.
        /// </summary>
        Uri TumblrUrl { get; }
    }
}
