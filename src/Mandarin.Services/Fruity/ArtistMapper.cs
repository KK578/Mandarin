using System;
using Mandarin.Models.Artists;

namespace Mandarin.Services.Fruity
{
    /// <summary>
    /// Represents the converter between Artist related DTOs to Domain models.
    /// </summary>
    internal static class ArtistMapper
    {
        private const string FacebookFormat = "https://facebook.com/{0}";
        private const string TwitterFormat = "https://twitter.com/{0}";
        private const string InstagramFormat = "https://instagram.com/{0}";
        private const string TumblrFormat = "https://{0}.tumblr.com/";

        /// <summary>
        /// Converts the provided <see cref="ArtistDto"/> to an <see cref="ArtistDetailsModel"/>.
        /// </summary>
        /// <param name="dto">The artist DTO to convert.</param>
        /// <returns>A newly created <see cref="ArtistDetailsModel"/>.</returns>
        public static ArtistDetailsModel ConvertToModel(ArtistDto dto)
        {
            return new ArtistDetailsModel(dto.StockistCode,
                                          dto.StockistName,
                                          dto.Description,
                                          decimal.Divide(dto.Rate, 100),
                                          dto.EmailAddress,
                                          ArtistMapper.MapUri(dto.ImageUrl),
                                          ArtistMapper.MapUri(dto.TwitterHandle, ArtistMapper.TwitterFormat),
                                          ArtistMapper.MapUri(dto.InstagramHandle, ArtistMapper.InstagramFormat),
                                          ArtistMapper.MapUri(dto.FacebookHandle, ArtistMapper.FacebookFormat),
                                          ArtistMapper.MapUri(dto.TumblrHandle, ArtistMapper.TumblrFormat),
                                          ArtistMapper.MapUri(dto.WebsiteUrl));
        }

        private static Uri MapUri(string uri, string format = null)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return null;
            }

            return string.IsNullOrEmpty(format) ? new Uri(uri) : new Uri(string.Format(format, uri));
        }
    }
}
