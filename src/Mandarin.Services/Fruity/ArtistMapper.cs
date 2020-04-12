﻿using System;
using Mandarin.Models.Artists;

namespace Mandarin.Services.Fruity
{
    // TODO KK: Consider AutoMapper.
    internal static class ArtistMapper
    {
        private const string FacebookFormat = "https://facebook.com/{0}";
        private const string TwitterFormat = "https://twitter.com/{0}";
        private const string InstagramFormat = "https://instagram.com/{0}";
        private const string TumblrFormat = "https://{0}.tumblr.com/";

        public static ArtistDetailsModel ConvertToModel(ArtistDto dto)
        {
            return new ArtistDetailsModel(dto.StockistName,
                                          dto.Description,
                                          new Uri(dto.ImageUrl),
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
