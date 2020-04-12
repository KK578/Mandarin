using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace Mandarin.Tests.Pages
{
    [TestFixture("Development")]
    [TestFixture("Production")]
    public class GetPageIntegrationTests
    {
        private readonly WebApplicationFactory<MandarinStartup> factory;
        private HttpClient client;

        public GetPageIntegrationTests(string environment)
        {
            this.factory = new WebApplicationFactory<MandarinStartup>()
                .WithWebHostBuilder(b => b.UseEnvironment(environment));
        }

        [SetUp]
        public void SetUp()
        {
            this.client = this.factory.CreateClient();
        }

        [Test]
        [TestCase("/", "We hope to see you soon!")]
        [TestCase("/the-mini-mandarin", "The Mini Mandarin also has a range of sweet snacks and drinks from Asia to enjoy!")]
        [TestCase("/contact", "Feel free to contact us through this form and we will get back to you as soon as we can.")]
        public async Task BasicRenderTest_ShouldBeAbleToRenderRoute_AndFindSimpleStaticContentOnPage(
            string route,
            string expected)
        {
            var document = await WhenGetPage(route);
            Assert.That(document.DocumentElement.TextContent, Contains.Substring(expected));
        }

        private async Task<IDocument> WhenGetPage(string path)
        {
            var response = await this.client.GetAsync(path);
            var content = await response.Content.ReadAsStringAsync();
            Assert.That(() => response.EnsureSuccessStatusCode(), Throws.Nothing);
            Assert.That(response.Content.Headers.ContentType.MediaType, Contains.Substring("text/html"));
            var document = await BrowsingContext.New().OpenAsync(req => req.Content(content));
            return document;
        }
    }
}
