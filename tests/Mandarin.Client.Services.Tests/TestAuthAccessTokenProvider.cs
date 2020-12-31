using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Mandarin.Client.Services.Tests
{
    internal sealed class TestAuthAccessTokenProvider : IAccessTokenProvider
    {
        public ValueTask<AccessTokenResult> RequestAccessToken()
        {
            var token = new AccessToken { Value = "AuthorizedToken" };
            var result = new AccessTokenResult(AccessTokenResultStatus.Success, token, null);
            return ValueTask.FromResult(result);
        }

        public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            return this.RequestAccessToken();
        }
    }
}
