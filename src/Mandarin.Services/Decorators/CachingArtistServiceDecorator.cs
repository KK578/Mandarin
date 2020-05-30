using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LazyCache;
using Mandarin.Models.Artists;
using Microsoft.Extensions.Caching.Memory;

namespace Mandarin.Services.Decorators
{
    internal sealed class CachingArtistServiceDecorator : IArtistService
    {
        private const string CacheKey = nameof(IArtistService) + "." + nameof(CachingArtistServiceDecorator.GetArtistDetailsAsync);
        private readonly IArtistService artistService;
        private readonly IAppCache appCache;

        public CachingArtistServiceDecorator(IArtistService artistService, IAppCache appCache)
        {
            this.artistService = artistService;
            this.appCache = appCache;
        }

        public Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetailsAsync()
        {
            return this.appCache.GetOrAddAsync(CachingArtistServiceDecorator.CacheKey, CreateEntry);

            async Task<IReadOnlyList<ArtistDetailsModel>> CreateEntry(ICacheEntry e)
            {
                try
                {
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return await this.artistService.GetArtistDetailsAsync();
                }
                catch (Exception)
                {
                    e.AbsoluteExpiration = DateTimeOffset.MinValue;
                    return new List<ArtistDetailsModel>().AsReadOnly();
                }
            }
        }
    }
}
