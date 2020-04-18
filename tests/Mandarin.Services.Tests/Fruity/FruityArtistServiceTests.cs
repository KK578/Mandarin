using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Mandarin.Services.Fruity;
using Mandarin.Tests.Data;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Fruity
{
    [TestFixture]
    public class FruityArtistServiceTests
    {
        private HttpClient httpClient;

        [Test]
        public async Task GetArtistDetailsAsync_GivenInactiveArtistDataFromService_ReturnsEmptyList()
        {
            this.GivenFruityClientReturns(WellKnownTestData.Fruity.Stockist.InactiveArtistData);
            var artistDetails = await this.WhenGetArtistDetail();
            Assert.That(artistDetails, Has.Count.Zero);
        }

        [Test]
        public async Task GetArtistDetailsAsync_GivenMinimalJsonDataFromService_ShouldDeserializeCorrectly()
        {
            this.GivenFruityClientReturns(WellKnownTestData.Fruity.Stockist.MinimalArtistData);
            var artistDetails = await this.WhenGetArtistDetail();
            Assert.That(artistDetails, Has.Count.EqualTo(1));
            Assert.That(artistDetails[0].Name, Is.EqualTo("Artist Name"));
            Assert.That(artistDetails[0].Description, Is.EqualTo("Artist's Description."));
            Assert.That(artistDetails[0].ImageUrl, Is.EqualTo(new Uri("https://localhost/static/images/artist1.jpg")));
            Assert.That(artistDetails[0].TwitterUrl, Is.Null);
            Assert.That(artistDetails[0].InstagramUrl, Is.Null);
            Assert.That(artistDetails[0].FacebookUrl, Is.Null);
            Assert.That(artistDetails[0].TumblrUrl, Is.Null);
            Assert.That(artistDetails[0].WebsiteUrl, Is.Null);
        }

        [Test]
        public async Task GetArtistDetailsAsync_GivenJsonDataFromService_ShouldDeserializeCorrectly()
        {
            this.GivenFruityClientReturns(WellKnownTestData.Fruity.Stockist.FullArtistData);
            var artistDetails = await this.WhenGetArtistDetail();
            Assert.That(artistDetails, Has.Count.EqualTo(1));
            Assert.That(artistDetails[0].Name, Is.EqualTo("Artist Name"));
            Assert.That(artistDetails[0].Description, Is.EqualTo("Artist's Description."));
            Assert.That(artistDetails[0].ImageUrl, Is.EqualTo(new Uri("https://localhost/static/images/artist1.jpg")));
            Assert.That(artistDetails[0].TwitterUrl, Is.EqualTo(new Uri("https://twitter.com/ArtistTwitter")));
            Assert.That(artistDetails[0].InstagramUrl, Is.EqualTo(new Uri("https://instagram.com/ArtistInstagram")));
            Assert.That(artistDetails[0].FacebookUrl, Is.EqualTo(new Uri("https://facebook.com/ArtistFacebook")));
            Assert.That(artistDetails[0].TumblrUrl, Is.EqualTo(new Uri("https://ArtistTumblr.tumblr.com/")));
            Assert.That(artistDetails[0].WebsiteUrl, Is.EqualTo(new Uri("https://localhost/artist/website")));
        }

        private void GivenFruityClientReturns(string filename)
        {
            var data = File.ReadAllText(filename);
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                   .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(data) });
            this.httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost:9090") };
        }

        private Task<IReadOnlyList<ArtistDetailsModel>> WhenGetArtistDetail()
        {
            var subject = new FruityArtistService(this.httpClient);
            return subject.GetArtistDetailsAsync();
        }
    }
}
