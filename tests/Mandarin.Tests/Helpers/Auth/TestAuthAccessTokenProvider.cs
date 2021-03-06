﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Mandarin.Tests.Helpers.Auth
{
    public sealed class TestAuthAccessTokenProvider : IAccessTokenProvider
    {
        public ValueTask<AccessTokenResult> RequestAccessToken()
        {
            var token = new AccessToken { Value = TestAuthHandler.AuthorizedToken.Parameter };
            var result = new AccessTokenResult(AccessTokenResultStatus.Success, token, null);
            return ValueTask.FromResult(result);
        }

        public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            return this.RequestAccessToken();
        }
    }
}
