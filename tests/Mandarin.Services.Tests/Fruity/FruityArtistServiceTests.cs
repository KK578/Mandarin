using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Mandarin.Configuration;
using Mandarin.Models.Artists;
using Mandarin.Services.Fruity;
using Mandarin.Tests.Data;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Fruity
{
    [TestFixture]
    public class FruityArtistServiceTests
    {
        private HttpClient httpClient;
        private MandarinConfiguration mandarinConfiguration;

        [Test]
        public async Task GetArtistDetailsAsync_GivenInactiveArtistDataFromService_ReturnsEmptyList()
        {
            this.GivenMandarinConfigurationIsEmpty();
            this.GivenFruityClientReturns(WellKnownTestData.Fruity.Stockist.InactiveArtistData);
            var subject = new FruityArtistService(this.httpClient, Options.Create(this.mandarinConfiguration));
            var artistDetails = await subject.GetArtistDetailsAsync();
            Assert.That(artistDetails, Has.Count.Zero);
        }

        [Test]
        public async Task GetArtistDetailsAsync_GivenMinimalJsonDataFromService_ShouldDeserializeCorrectly()
        {
            this.GivenMandarinConfigurationIsEmpty();
            this.GivenFruityClientReturns(WellKnownTestData.Fruity.Stockist.MinimalArtistData);
            var subject = new FruityArtistService(this.httpClient, Options.Create(this.mandarinConfiguration));
            var artistDetails = await subject.GetArtistDetailsAsync();
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
            this.GivenMandarinConfigurationIsEmpty();
            this.GivenFruityClientReturns(WellKnownTestData.Fruity.Stockist.FullArtistData);
            var subject = new FruityArtistService(this.httpClient, Options.Create(this.mandarinConfiguration));
            var artistDetails = await subject.GetArtistDetailsAsync();
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

        [Test]
        public async Task GetArtistDetailsForCommissionAsync_GivenConfigurationContainsAdditionalValues_ShouldContainAllValues()
        {
            var artist = TestData.Create<ArtistDetailsModel>();
            this.GivenMandarinConfigurationHasValue(artist);
            this.GivenFruityClientReturns(WellKnownTestData.Fruity.Stockist.FullArtistData);
            var subject = new FruityArtistService(this.httpClient, Options.Create(this.mandarinConfiguration));
            var artistDetails = await subject.GetArtistDetailsForCommissionAsync();
            Assert.That(artistDetails, Has.Exactly(2).Items);
            Assert.That(artistDetails.Last().StockistCode, Is.EqualTo(artist.StockistCode));
        }

        private void GivenMandarinConfigurationIsEmpty()
        {
            this.mandarinConfiguration = new MandarinConfiguration
            {
                AdditionalStockists = new List<Dictionary<string, object>>(),
            };
        }

        private void GivenMandarinConfigurationHasValue(ArtistDetailsModel artist)
        {
            this.mandarinConfiguration = new MandarinConfiguration
            {
                AdditionalStockists = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { nameof(ArtistDetailsModel.StockistCode), artist.StockistCode },
                    },
                },
            };
        }

        private void GivenFruityClientReturns(string filename)
        {
            var data = File.ReadAllText(filename);
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                   .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                     ItExpr.IsAny<HttpRequestMessage>(),
                                                     ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(data) });
            this.httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost:9090") };
        }
    }
}
