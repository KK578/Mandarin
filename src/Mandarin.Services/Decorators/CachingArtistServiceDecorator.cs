﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LazyCache;
using Mandarin.Models.Artists;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Decorators
{
    /// <summary>
    /// Decorated implementation of <see cref="IArtistService"/> that caches results to speed up subsequent requests.
    /// </summary>
    internal sealed class CachingArtistServiceDecorator : CachingDecoratorBase, IArtistService
    {
        private readonly IArtistService artistService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingArtistServiceDecorator"/> class.
        /// </summary>
        /// <param name="artistService">The artist service to be decorated.</param>
        /// <param name="appCache">The application memory cache.</param>
        /// <param name="logger">The application logger.</param>
        public CachingArtistServiceDecorator(IArtistService artistService, IAppCache appCache, ILogger<CachingArtistServiceDecorator> logger)
            : base(appCache, logger)
        {
            this.artistService = artistService;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetailsAsync()
        {
            try
            {
                return await this.GetOrAddAsync(this.CreateCacheKey(), async () => (await this.artistService.GetArtistDetailsAsync()).AsEnumerable());
            }
            catch (Exception)
            {
                return new List<ArtistDetailsModel>().AsReadOnly();
            }
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetailsForCommissionAsync()
        {
            try
            {
                return await this.GetOrAddAsync(this.CreateCacheKey(), async () => (await this.artistService.GetArtistDetailsForCommissionAsync()).AsEnumerable());
            }
            catch (Exception)
            {
                return new List<ArtistDetailsModel>().AsReadOnly();
            }
        }

        private string CreateCacheKey([CallerMemberName] string caller = null)
        {
            return nameof(IArtistService) + "." + caller;
        }
    }
}
