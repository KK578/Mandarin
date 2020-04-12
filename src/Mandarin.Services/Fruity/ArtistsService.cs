using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Newtonsoft.Json;

namespace Mandarin.Services.Fruity
{
    internal sealed class FruityArtistsService : IArtistsService
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializer serializer;

        public FruityArtistsService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.serializer = new JsonSerializer();
        }

        public async Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetails()
        {
            var response = await this.httpClient.GetAsync("/api/stockists");
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);
            var results = this.serializer.Deserialize<List<ArtistDetailsModel>>(jsonReader);
            return results.AsReadOnly();
        }
    }
}
