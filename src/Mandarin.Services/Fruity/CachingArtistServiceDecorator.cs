using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Microsoft.Extensions.Caching.Memory;

namespace Mandarin.Services.Fruity
{
    internal sealed class CachingArtistServiceDecorator : IArtistService
    {
        private const string CacheKey = "IArtistService.GetArtistDetailsAsync";
        private readonly IArtistService artistService;
        private readonly IMemoryCache memoryCache;

        public CachingArtistServiceDecorator(IArtistService artistService, IMemoryCache memoryCache)
        {
            this.artistService = artistService;
            this.memoryCache = memoryCache;
        }

        public Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetailsAsync()
        {
            return this.memoryCache.GetOrCreateAsync(CachingArtistServiceDecorator.CacheKey, CreateEntry);

            async Task<IReadOnlyList<ArtistDetailsModel>> CreateEntry(ICacheEntry e)
            {
                try
                {
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return await this.artistService.GetArtistDetailsAsync();
                }
                catch (Exception ex)
                {
                    e.AbsoluteExpiration = DateTimeOffset.MinValue;
                    return new List<ArtistDetailsModel>().AsReadOnly();
                }
            }
        }
    }
}
