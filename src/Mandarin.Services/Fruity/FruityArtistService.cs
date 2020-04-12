using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Newtonsoft.Json;

namespace Mandarin.Services.Fruity
{
    internal sealed class FruityArtistService : IArtistService
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializer serializer;

        public FruityArtistService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.serializer = new JsonSerializer();
        }

        public async Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetailsAsync()
        {
            var response = await this.httpClient.GetAsync("/api/stockists");
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);
            var results = this.serializer.Deserialize<List<ArtistDto>>(jsonReader);
            var models = results.Select(ArtistMapper.ConvertToModel).ToList().AsReadOnly();
            return models;
        }
    }
}
