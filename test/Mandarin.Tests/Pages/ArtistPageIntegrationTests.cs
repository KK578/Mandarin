using System.Collections.Generic;
using System.Linq;
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
        private Mock<IArtistService> artistService;
        private const string ArtistName = "Artist Name";

        public ArtistPageIntegrationTests()
        {
            this.factory = new WebApplicationFactory<MandarinStartup>();
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
            var tcs = GivenArtistServiceWaitsForReturn();
            var artistPage = await this.WhenGetArtistPage();
            Assert.That(artistPage.DocumentElement.TextContent, Contains.Substring("Just a moment..."));
            tcs.SetCanceled();
        }

        [Test]
        public async Task GetArtists_WhenDataIsAvailable_ShouldRenderRegisteredArtistViewModels()
        {
            GivenArtistServiceReturnsDataImmediately();
            var artistPage = await this.WhenGetArtistPage();
            Assert.That(artistPage.DocumentElement.TextContent, Contains.Substring(ArtistPageIntegrationTests.ArtistName));
            Assert.That(artistPage.DocumentElement.TextContent, Contains.Substring(TestData.WellKnownString));
        }

        private TaskCompletionSource<IReadOnlyList<ArtistDetailsModel>> GivenArtistServiceWaitsForReturn()
        {
            var tcs = new TaskCompletionSource<IReadOnlyList<ArtistDetailsModel>>();
            this.artistService.Setup(x => x.GetArtistDetailsAsync()).Returns(tcs.Task);
            return tcs;
        }


        private void GivenArtistServiceReturnsDataImmediately()
        {
            var data = TestData.Create<List<ArtistDetailsModel>>()
                               .Append(new ArtistDetailsModel(ArtistPageIntegrationTests.ArtistName, TestData.WellKnownString, null, null, null, null, null, null))
                               .ToList()
                               .AsReadOnly();
            this.artistService.Setup(x => x.GetArtistDetailsAsync()).ReturnsAsync(data);
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
