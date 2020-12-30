using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Mandarin.Client.Services.Authentication
{
    /// <summary>
    /// HttpHandler which attaches the current user's JWT token to the outgoing request.
    /// </summary>
    internal sealed class JwtHttpMessageHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider accessTokenProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtHttpMessageHandler"/> class.
        /// </summary>
        /// <param name="accessTokenProvider">Provider for accessing the current JWT Tokens for the user session.</param>
        public JwtHttpMessageHandler(IAccessTokenProvider accessTokenProvider)
        {
            this.accessTokenProvider = accessTokenProvider;
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tokenResult = await this.accessTokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out var token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
