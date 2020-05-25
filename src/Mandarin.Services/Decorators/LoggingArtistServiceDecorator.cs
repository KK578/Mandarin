using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Decorators
{
    internal sealed class LoggingArtistServiceDecorator : IArtistService
    {
        private readonly IArtistService artistService;
        private readonly ILogger<IArtistService> logger;

        public LoggingArtistServiceDecorator(IArtistService artistService, ILogger<IArtistService> logger)
        {
            this.artistService = artistService;
            this.logger = logger;
        }

        public async Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetailsAsync()
        {
            try
            {
                return await this.artistService.GetArtistDetailsAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred whilst fetching Artist Details.");
                throw;
            }
        }
    }
}
