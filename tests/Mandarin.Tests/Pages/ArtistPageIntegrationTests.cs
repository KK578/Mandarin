using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Bashi.Tests.Framework.Data;
using Mandarin.Models.Artists;
using Mandarin.Services;
using Mandarin.Tests.Factory;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Mandarin.Tests.Pages
{
    [TestFixture]
    public class ArtistPageIntegrationTests
    {
        private const string ArtistName = "Artist Name";

        private readonly WebApplicationFactory<MandarinStartup> factory;
        private HttpClient client;
        private Mock<IArtistService> artistService;

        public ArtistPageIntegrationTests()
        {
            this.factory = MandarinApplicationFactory.Create();
        }

        [SetUp]
        public void SetUp()
        {
            this.artistService = new Mock<IArtistService>();
            this.client = this.factory.WithWebHostBuilder(b => b.ConfigureServices(RegisterServices)).CreateClient();

            void RegisterServices(IServiceCollection s)
            {
                s.AddSingleton(this.artistService.Object);
            }
        }

        [Test]
        public async Task GetArtists_WhenDataIsLoading_ShouldRenderLoadingText()
        {
            var tcs = this.GivenArtistServiceWaitsForReturn();
            var artistPage = await this.WhenGetArtistPage();
            Assert.That(artistPage.DocumentElement.TextContent, Contains.Substring("Just a moment..."));
            tcs.SetCanceled();
        }

        [Test]
        public async Task GetArtists_WhenDataIsAvailable_ShouldRenderRegisteredArtistViewModels()
        {
            this.GivenArtistServiceReturnsDataImmediately();
            var artistPage = await this.WhenGetArtistPage();
            Assert.That(artistPage.DocumentElement.TextContent, Contains.Substring(ArtistPageIntegrationTests.ArtistName));
            Assert.That(artistPage.DocumentElement.TextContent, Contains.Substring(TestData.WellKnownString));
        }

        private TaskCompletionSource<Stockist> GivenArtistServiceWaitsForReturn()
        {
            var tcs = new TaskCompletionSource<Stockist>();
            this.artistService.Setup(x => x.GetArtistsForDisplayAsync()).Returns(tcs.Task.ToObservable());
            return tcs;
        }


        private void GivenArtistServiceReturnsDataImmediately()
        {
            var data = new Stockist
            {
                ShortDisplayName = ArtistPageIntegrationTests.ArtistName,
                Description = TestData.WellKnownString,
                Details = new StockistDetail(),
            };

            this.artistService.Setup(x => x.GetArtistsForDisplayAsync()).Returns(Observable.Return(data));
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
