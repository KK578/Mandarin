using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.Artists;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Mandarin.Tests.Pages
{
    [TestFixture]
    public class ArtistPageIntegrationTests
    {
        private readonly WebApplicationFactory<MandarinStartup> factory;
        private HttpClient client;
        private IArtistViewModel artistViewModel;

        public ArtistPageIntegrationTests()
        {
            this.factory = new WebApplicationFactory<MandarinStartup>();
        }

        [SetUp]
        public void SetUp()
        {
            this.artistViewModel = new ArtistViewModel
            {
                Name = "My Artist Name",
                Description = TestData.WellKnownString,
            };
            this.client = this.factory.WithWebHostBuilder(b => b.ConfigureServices(RegisterViewModels)).CreateClient();

            void RegisterViewModels(IServiceCollection s)
            {
                s.AddSingleton(this.artistViewModel);
                s.AddSingleton<IArtistViewModel>(TestData.Create<ArtistViewModel>());
            }
        }

        [Test]
        public async Task GetArtists_ShouldRenderRegisteredArtistViewModels()
        {
            var artistPage = await this.WhenGetArtistPage();
            Assert.That(artistPage.DocumentElement.TextContent, Contains.Substring(this.artistViewModel.Name));
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
