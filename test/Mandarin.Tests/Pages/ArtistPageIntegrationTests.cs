using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Bashi.Tests.Framework.Data;
using Mandarin.Models.Artists;
using Mandarin.Services.Fruity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Mandarin.Tests.Pages
{
    [TestFixture]
    public class ArtistPageIntegrationTests
    {
        private readonly WebApplicationFactory<MandarinStartup> factory;
        private HttpClient client;
        private const string ArtistName = "Artist Name";

        public ArtistPageIntegrationTests()
        {
            this.factory = new WebApplicationFactory<MandarinStartup>();
        }

        [SetUp]
        public void SetUp()
        {
            var model = new ArtistDetailsModel
            {
                Name = ArtistPageIntegrationTests.ArtistName,
                Description = TestData.WellKnownString,
            };
            var data = new List<ArtistDetailsModel> { model }.AsReadOnly();
            var artistsService = new Mock<IArtistsService>();
            artistsService.Setup(x => x.GetArtistDetails()).ReturnsAsync(data);
            this.client = this.factory.WithWebHostBuilder(b => b.ConfigureServices(RegisterServices)).CreateClient();

            void RegisterServices(IServiceCollection s)
            {
                s.AddSingleton(artistsService.Object);
            }
        }

        [Test]
        public async Task GetArtists_ShouldRenderRegisteredArtistViewModels()
        {
            var artistPage = await this.WhenGetArtistPage();
            Assert.That(artistPage.DocumentElement.TextContent, Contains.Substring(ArtistPageIntegrationTests.ArtistName));
            Assert.That(artistPage.DocumentElement.TextContent, Contains.Substring(TestData.WellKnownString));
        }

        private async Task<IDocument> WhenGetArtistPage()
        {
            var response = await this.client.GetAsync("/artists");
            var content = await response.Content.ReadAsStringAsync();
            var document = await BrowsingContext.New().OpenAsync(req => req.Content(content));
            return document;
        }
    }
}
