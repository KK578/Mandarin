using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Mandarin.Models.Artists;

namespace Mandarin.App.Client
{
    /// <summary>
    /// Wrapper for <see cref="HttpClient" /> for calling Mandarin API features.
    /// </summary>
    internal sealed class MandarinHttpClient
    {
        private static readonly JsonSerializerOptions JsonSerializationOptions = new()
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinHttpClient"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> instance for creating a mandarin <see cref="HttpClient"/> instance.</param>
        public MandarinHttpClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Sends a GET request to the specified URI and return the value resulting from deserializing the response body
        /// as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response as.</typeparam>
        /// <param name="requestUri">The relative path of the resource to be fetched.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the deserialized response from the Mandarin API.</returns>
        public Task<T> GetAsync<T>(string requestUri)
        {
            var httpClient = this.httpClientFactory.CreateClient("Mandarin");
            return httpClient.GetFromJsonAsync<T>(requestUri, MandarinHttpClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Sends a POST request to the specified URI containing the value serialized as JSON in the request body.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="value"/> to be serialized.</typeparam>
        /// <param name="requestUri">The relative path of the resource to be fetched.</param>
        /// <param name="value">The value to be serialized.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PostAsync<T>(string requestUri, T value)
        {
            var httpClient = this.httpClientFactory.CreateClient("Mandarin");
            return httpClient.PostAsJsonAsync(requestUri, value, MandarinHttpClient.JsonSerializationOptions);
        }
    }
}
