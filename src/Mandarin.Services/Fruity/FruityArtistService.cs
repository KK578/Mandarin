using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mandarin.Configuration;
using Mandarin.Models.Artists;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mandarin.Services.Fruity
{
    /// <inheritdoc />
    internal sealed class FruityArtistService : IArtistService
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<MandarinConfiguration> mandarinConfiguration;
        private readonly JsonSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FruityArtistService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client configured for communicating with Fruity service.</param>
        /// <param name="mandarinConfiguration">The application configuration.</param>
        public FruityArtistService(HttpClient httpClient, IOptions<MandarinConfiguration> mandarinConfiguration)
        {
            this.httpClient = httpClient;
            this.mandarinConfiguration = mandarinConfiguration;
            this.serializer = new JsonSerializer();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetailsAsync()
        {
            var response = await this.httpClient.GetAsync("/api/stockist");
            var results = await this.Deserialize<List<ArtistDto>>(response);
            var models = results.Where(x => string.Equals(x.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                                .Select(ArtistMapper.ConvertToModel).ToList().AsReadOnly();
            return models;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetailsForCommissionAsync()
        {
            var artists = await this.GetArtistDetailsAsync();
            var additionalArtists = this.GetAdditionalArtistDetails();
            return artists.Concat(additionalArtists)
                          .OrderBy(x => x.StockistCode)
                          .ToList()
                          .AsReadOnly();
        }

        private IEnumerable<ArtistDetailsModel> GetAdditionalArtistDetails()
        {
            return JArray.FromObject(this.mandarinConfiguration.Value.AdditionalStockists)
                         .ToObject<List<ArtistDto>>()
                         ?.Select(ArtistMapper.ConvertToModel);
        }

        private async Task<T> Deserialize<T>(HttpResponseMessage response)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);
            var results = this.serializer.Deserialize<T>(jsonReader);
            return results;
        }
    }
}
