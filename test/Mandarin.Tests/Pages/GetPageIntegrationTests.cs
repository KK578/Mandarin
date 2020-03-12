using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.Artists;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Mandarin.Tests.Pages
{
    [TestFixture("Development")]
    [TestFixture("Production")]
    public class GetPageIntegrationTests
    {
        private readonly WebApplicationFactory<MandarinStartup> factory;

        public GetPageIntegrationTests(string environment)
        {
            this.factory = new WebApplicationFactory<MandarinStartup>()
                .WithWebHostBuilder(b => b.UseEnvironment(environment));
        }

        [Test]
        public async Task GetIndex_ShouldRenderHomePage()
        {
            // Arrange
            var client = this.factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            Assert.That(() => response.EnsureSuccessStatusCode(), Throws.Nothing);
            Assert.That(response.Content.Headers.ContentType.MediaType, Contains.Substring("text/html"));

            var pageResponse = await response.Content.ReadAsStringAsync();
            Assert.That(pageResponse, Contains.Substring("We hope to see you soon!"));
        }

        [Test]
        public async Task GetMiniMandarin_ShouldRenderMiniMandarinPage()
        {
            // Arrange
            var client = this.factory.CreateClient();

            // Act
            var response = await client.GetAsync("/the-mini-mandarin");

            // Assert
            Assert.That(() => response.EnsureSuccessStatusCode(), Throws.Nothing);
            Assert.That(response.Content.Headers.ContentType.MediaType, Contains.Substring("text/html"));

            var pageResponse = await response.Content.ReadAsStringAsync();
            Assert.That(pageResponse, Contains.Substring("bear shaped macarons!"));
        }

        [Test]
        public async Task GetArtists_ShouldRenderArtistsPage()
        {
            // Arrange
            var viewModel = new ArtistViewModel
            {
                Name = "My Artist Name",
                Description = TestData.WellKnownString
            };
            var client = this.factory.WithWebHostBuilder(b => b.ConfigureServices(s => s.AddSingleton(viewModel)))
                             .CreateClient();

            // Act
            var response = await client.GetAsync("/artists");

            // Assert
            Assert.That(() => response.EnsureSuccessStatusCode(), Throws.Nothing);
            Assert.That(response.Content.Headers.ContentType.MediaType, Contains.Substring("text/html"));

            var pageResponse = await response.Content.ReadAsStringAsync();
            Assert.That(pageResponse, Contains.Substring("My Artist Name"));
            Assert.That(pageResponse, Contains.Substring(TestData.WellKnownString));
        }
    }
}
