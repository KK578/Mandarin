using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Newtonsoft.Json;

namespace Mandarin.Services.Fruity
{
    /// <inheritdoc />
    internal sealed class FruityArtistService : IArtistService
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FruityArtistService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client configured for communicating with Fruity service.</param>
        public FruityArtistService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.serializer = new JsonSerializer();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetailsAsync()
        {
            // TODO: Additional artists may be required for commission.
            var response = await this.httpClient.GetAsync("/api/stockist");
            var results = await this.Deserialize<List<ArtistDto>>(response);
            var models = results.Where(x => string.Equals(x.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                                .Select(ArtistMapper.ConvertToModel).ToList().AsReadOnly();
            return models;
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
