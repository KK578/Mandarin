using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Decorators
{
    /// <inheritdoc />
    internal sealed class LoggingArtistServiceDecorator : IArtistService
    {
        private readonly IArtistService artistService;
        private readonly ILogger<IArtistService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingArtistServiceDecorator"/> class.
        /// </summary>
        /// <param name="artistService">The artist service to be decorated.</param>
        /// <param name="logger">The application to log to.</param>
        public LoggingArtistServiceDecorator(IArtistService artistService, ILogger<IArtistService> logger)
        {
            this.artistService = artistService;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public IObservable<Stockist> GetArtistsForDisplayAsync()
        {
            try
            {
                // TODO: Seems redundant.
                return this.artistService.GetArtistsForDisplayAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred whilst fetching Artist Details.");
                throw;
            }
        }

        /// <inheritdoc/>
        public IObservable<Stockist> GetArtistsForCommissionAsync()
        {
            try
            {
                return this.artistService.GetArtistsForCommissionAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred whilst fetching Artist Details.");
                throw;
            }
        }
    }
}
