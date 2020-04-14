using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Mandarin.Services.Fruity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Mandarin.Tests.Pages
{
    [TestFixture("Development")]
    [TestFixture("Production")]
    public class GetPageIntegrationTests
    {
        private readonly WebApplicationFactory<MandarinStartup> factory;
        private HttpClient client;
        private WireMockServer fruityMock;

        public GetPageIntegrationTests(string environment)
        {
            this.factory = new WebApplicationFactory<MandarinStartup>()
                .WithWebHostBuilder(b => b.UseEnvironment(environment));
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.fruityMock = WireMockServer.Start("http://localhost:9090");
            this.fruityMock
                .Given(Request.Create().WithPath("/api/stockist"))
                .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK).WithBodyFromFile("TestData/Fruity/Stockist/TheLittleMandarin.json"));
        }

        [SetUp]
        public void SetUp()
        {
            this.client = this.factory.CreateClient();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.fruityMock.Dispose();
        }

        [Test]
        [TestCase("/", "We hope to see you soon!")]
        [TestCase("/the-mini-mandarin", "The Mini Mandarin also offers a range of sweets and chocolates suitable for vegetarians and vegans - there's a treat for everyone!")]
        [TestCase("/artists", "The Little Mandarin in-house art team!")]
        [TestCase("/contact", "Feel free to contact us through this form and we will get back to you as soon as we can.")]
        public async Task BasicRenderTest_ShouldBeAbleToRenderRoute_AndFindSimpleStaticContentOnPage(
            string route,
            string expected)
        {
            if (route == "/artists")
            {
                await this.GivenArtistServiceHasPreCached();
            }

            var document = await WhenGetPage(route);
            Assert.That(document.DocumentElement.TextContent, Contains.Substring(expected));
        }

        private Task GivenArtistServiceHasPreCached()
        {
            var artistService = this.factory.Services.GetService<IArtistService>();
            Assert.That(artistService, Is.InstanceOf<CachingArtistServiceDecorator>());
            return artistService.GetArtistDetailsAsync();
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
