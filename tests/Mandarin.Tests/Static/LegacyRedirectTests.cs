using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Static
{
    [Collection(nameof(MandarinTestsCollectionFixture))]
    public class LegacyRedirectTests : MandarinIntegrationTestsBase
    {
        private readonly HttpClient client;

        public LegacyRedirectTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
            this.client = this.Fixture.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
        }

        [Theory]
        [InlineData("/static/logo-300.png", "/static/images/logo.png")]
        [InlineData("/static/Century-Schoolbook-Std-Regular.otf", "/static/fonts/Century-Schoolbook-Std-Regular.otf")]
        public async Task GetStaticPath_GivenLegacyPath_ShouldRedirect(string requestPath, string expectedRedirect)
        {
            var response = await this.client.GetAsync(requestPath);
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location.ToString().Should().Be(expectedRedirect);
        }
    }
}
