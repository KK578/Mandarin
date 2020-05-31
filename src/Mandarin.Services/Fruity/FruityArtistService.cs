﻿using System;
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
            // TODO: Allow config-based additions whilst the database does not handle some usecases.
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
