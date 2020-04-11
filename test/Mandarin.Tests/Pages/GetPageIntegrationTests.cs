using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.Artists;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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
        [TestCase("/", "We hope to see you soon!")]
        [TestCase("/the-mini-mandarin", "The Mini Mandarin also has a range of sweet snacks and drinks from Asia to enjoy!")]
        [TestCase("/contact", "Feel free to contact us through this form and we will get back to you as soon as we can.")]
        public async Task BasicRenderTest_ShouldBeAbleToRenderRoute_AndFindSimpleStaticContentOnPage(
            string route,
            string expected)
        {
            // Arrange
            var client = this.factory.CreateClient();

            // Act
            var response = await client.GetAsync(route);

            // Assert
            Assert.That(() => response.EnsureSuccessStatusCode(), Throws.Nothing);
            Assert.That(response.Content.Headers.ContentType.MediaType, Contains.Substring("text/html"));

            var pageContent = await response.Content.ReadAsStringAsync();
            Assert.That(pageContent, Contains.Substring(expected));
        }

        [Test]
        public async Task GetArtists_ShouldRenderArtistsPage()
        {
            // Arrange
            var client = this.factory.WithWebHostBuilder(b => b.ConfigureServices(RegisterViewModels)).CreateClient();

            // Act
            var response = await client.GetAsync("/artists");

            // Assert
            Assert.That(() => response.EnsureSuccessStatusCode(), Throws.Nothing);
            Assert.That(response.Content.Headers.ContentType.MediaType, Contains.Substring("text/html"));

            var pageResponse = await response.Content.ReadAsStringAsync();
            Assert.That(pageResponse, Contains.Substring("My Artist Name"));
            Assert.That(pageResponse, Contains.Substring(TestData.WellKnownString));


            static void RegisterViewModels(IServiceCollection s)
            {
                s.AddSingleton<IArtistViewModel>(new ArtistViewModel
                {
                    Name = "My Artist Name",
                    Description = TestData.WellKnownString,
                });
                s.AddSingleton<IArtistViewModel>(TestData.Create<ArtistViewModel>());
            }
        }
    }
}
