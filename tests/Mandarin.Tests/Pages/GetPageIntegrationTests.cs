using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Tests.Helpers;
using Mandarin.Tests.Helpers.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Pages
{
    [Collection(nameof(MandarinTestsCollectionFixture))]
    public class GetPageIntegrationTests : MandarinIntegrationTestsBase
    {
        private readonly HttpClient client;

        private HttpRequestMessage request;
        private HttpResponseMessage response;

        public GetPageIntegrationTests(MandarinTestFixture mandarinTestFixture, ITestOutputHelper testOutputHelper)
            : base(mandarinTestFixture, testOutputHelper)
        {
            this.client = this.Fixture.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
        }

        [Fact]
        public async Task AuthenticationTest_WhenNotAuthenticated_ShouldRedirectToAboutPage()
        {
            this.GivenUnauthenticatedRequestFor("/");
            await this.WhenPageResponseIsRequested();
            this.AssertResponseIsRedirectedTo("/login");
        }

        [Fact]
        public async Task AuthenticationTest_WhenAuthenticated_ShouldRedirectToAdminPage()
        {
            this.GivenAuthenticatedRequestFor("/");
            await this.WhenPageResponseIsRequested();
            this.AssertResponseIsSuccessfulAndHtml();
        }

        private void GivenUnauthenticatedRequestFor(string path)
        {
            this.request = new HttpRequestMessage(HttpMethod.Get, path);
        }

        private void GivenAuthenticatedRequestFor(string path)
        {
            this.request = new HttpRequestMessage(HttpMethod.Get, path);
            this.request.Headers.Authorization = TestAuthHandler.AuthorizedToken;
        }

        private async Task WhenPageResponseIsRequested()
        {
            this.response = await this.client.SendAsync(this.request);
        }

        private void AssertResponseIsSuccessfulAndHtml()
        {
            this.response.Invoking(x => x.EnsureSuccessStatusCode()).Should().NotThrow();
            this.response.Content.Headers.ContentType.MediaType.Should().Contain("text/html");
        }

        private void AssertResponseIsRedirectedTo(string expectedPath)
        {
            this.response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            this.response.Headers.Location.AbsolutePath.Should().Be(expectedPath);
        }
    }
}
