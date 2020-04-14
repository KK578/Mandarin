using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Mandarin.Services.Fruity;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Mandarin.Services.Tests.Fruity
{
    [TestFixture]
    public class FruityArtistServiceTests
    {
        private const string BaseAddress = "http://localhost:9090";

        private HttpClient httpClient;
        private WireMockServer fruityMock;

        [SetUp]
        public void SetUp()
        {
            this.fruityMock = WireMockServer.Start(FruityArtistServiceTests.BaseAddress);
            this.httpClient = new HttpClient { BaseAddress = new Uri(FruityArtistServiceTests.BaseAddress) };
        }

        [TearDown]
        public void TearDown()
        {
            this.fruityMock.Dispose();
        }

        [Test]
        public async Task GetArtistDetailsAsync_GivenMinimalJsonDataFromService_ShouldDeserializeCorrectly()
        {
            this.GivenFruityClientReturns("MinimalArtistData");
            var artistDetails = await this.WhenGetArtistDetail();
            Assert.That(artistDetails, Has.Count.EqualTo(1));
            Assert.That(artistDetails[0].Name, Is.EqualTo("Artist Name"));
            Assert.That(artistDetails[0].Description, Is.EqualTo("Artist's Description."));
            Assert.That(artistDetails[0].ImageUrl, Is.EqualTo(new Uri("https://localhost/images/artist1.jpg")));
            Assert.That(artistDetails[0].TwitterUrl, Is.Null);
            Assert.That(artistDetails[0].InstagramUrl, Is.Null);
            Assert.That(artistDetails[0].FacebookUrl, Is.Null);
            Assert.That(artistDetails[0].TumblrUrl, Is.Null);
            Assert.That(artistDetails[0].WebsiteUrl, Is.Null);
        }

        [Test]
        public async Task GetArtistDetailsAsync_GivenJsonDataFromService_ShouldDeserializeCorrectly()
        {
            this.GivenFruityClientReturns("FullArtistData");
            var artistDetails = await this.WhenGetArtistDetail();
            Assert.That(artistDetails, Has.Count.EqualTo(1));
            Assert.That(artistDetails[0].Name, Is.EqualTo("Artist Name"));
            Assert.That(artistDetails[0].Description, Is.EqualTo("Artist's Description."));
            Assert.That(artistDetails[0].ImageUrl, Is.EqualTo(new Uri("https://localhost/images/artist1.jpg")));
            Assert.That(artistDetails[0].TwitterUrl, Is.EqualTo(new Uri("https://twitter.com/ArtistTwitter")));
            Assert.That(artistDetails[0].InstagramUrl, Is.EqualTo(new Uri("https://instagram.com/ArtistInstagram")));
            Assert.That(artistDetails[0].FacebookUrl, Is.EqualTo(new Uri("https://facebook.com/ArtistFacebook")));
            Assert.That(artistDetails[0].TumblrUrl, Is.EqualTo(new Uri("https://ArtistTumblr.tumblr.com/")));
            Assert.That(artistDetails[0].WebsiteUrl, Is.EqualTo(new Uri("https://localhost/artist/website")));
        }

        private void GivenFruityClientReturns(string filename)
        {
            this.fruityMock
                .Given(Request.Create().WithPath("/api/stockist"))
                .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK).WithBodyFromFile($"TestData/Fruity/Stockist/{filename}.json"));
        }

        private Task<IReadOnlyList<ArtistDetailsModel>> WhenGetArtistDetail()
        {
            var subject = new FruityArtistService(this.httpClient);
            return subject.GetArtistDetailsAsync();
        }
    }
}
