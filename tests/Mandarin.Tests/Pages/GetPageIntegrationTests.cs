using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Mandarin.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace Mandarin.Tests.Pages
{
    [TestFixture("Development")]
    [TestFixture("Production")]
    public class GetPageIntegrationTests : MandarinIntegrationTestsBase
    {
        private readonly HttpClient client;

        private HttpRequestMessage request;
        private HttpResponseMessage response;

        public GetPageIntegrationTests(string environment)
        {
            this.client = this.Factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        }

        [Test]
        public async Task AuthenticationTest_WhenNotAuthenticated_ShouldRedirectToAboutPage()
        {
            this.GivenUnauthenticatedRequestFor("/");
            await this.WhenPageResponseIsRequested();
            this.AssertResponseIsRedirectedTo("/login");
        }

        [Test]
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

        private async Task<IDocument> WhenPageContentIsRequested()
        {
            await this.WhenPageResponseIsRequested();
            var content = await this.response.Content.ReadAsStringAsync();
            this.AssertResponseIsSuccessfulAndHtml();
            var document = await BrowsingContext.New().OpenAsync(req => req.Content(content));
            return document;
        }

        private void AssertResponseIsSuccessfulAndHtml()
        {
            Assert.That(this.response.EnsureSuccessStatusCode, Throws.Nothing);
            Assert.That(this.response.Content.Headers.ContentType.MediaType, Contains.Substring("text/html"));
        }

        private void AssertResponseIsRedirectedTo(string expectedPath)
        {
            Assert.That(this.response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            Assert.That(this.response.Headers.Location.AbsolutePath, Is.EqualTo(expectedPath));
        }
    }
}
