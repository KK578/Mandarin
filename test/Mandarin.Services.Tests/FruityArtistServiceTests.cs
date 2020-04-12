using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Mandarin.Services.Fruity;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Mandarin.Services.Tests
{
    [TestFixture]
    public class FruityArtistServiceTests
    {
        private const string MinimalArtistData = @"[{""stockist_name"": ""Artist Name"", ""description"": ""Artist's Description."", ""image_url"": ""https://localhost/images/artist1.jpg""}]";
        private const string FullArtistData = @"[{""stockist_name"": ""Artist Name"", ""description"": ""Artist's Description."", ""image_url"": ""https://localhost/images/artist1.jpg"", ""twitter_handle"": ""ArtistTwitter"", ""instagram_handle"": ""ArtistInstagram"", ""facebook_handle"": ""ArtistFacebook"", ""tumblr_handle"": ""ArtistTumblr"", ""website_url"": ""https://localhost/artist/website"" }]";

        private Mock<HttpMessageHandler> messageHandler;
        private HttpClient httpClient;

        [SetUp]
        public void SetUp()
        {
            this.messageHandler = new Mock<HttpMessageHandler>();
            this.httpClient = new HttpClient(this.messageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:9000")
            };
        }

        [Test]
        public async Task GetArtistDetail_GivenMinimalJsonDataFromService_ShouldDeserializeCorrectly()
        {
            GivenFruityClientReturns(FruityArtistServiceTests.MinimalArtistData);
            var artistDetails = await WhenGetArtistDetail();
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
        public async Task GetArtistDetail_GivenJsonDataFromService_ShouldDeserializeCorrectly()
        {
            GivenFruityClientReturns(FruityArtistServiceTests.FullArtistData);
            var artistDetails = await WhenGetArtistDetail();
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

        private void GivenFruityClientReturns(string data)
        {
            this.messageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(data)
                });
        }

        private Task<IReadOnlyList<ArtistDetailsModel>> WhenGetArtistDetail()
        {
            var subject = new FruityArtistService(this.httpClient);
            return subject.GetArtistDetails();
        }
    }
}
