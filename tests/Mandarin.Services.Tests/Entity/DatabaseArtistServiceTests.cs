﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Mandarin.Configuration;
using Mandarin.Models.Artists;
using Mandarin.Services.Entity;
using Mandarin.Services.Fruity;
using Mandarin.Tests.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Entity
{
    [TestFixture]
    public class DatabaseArtistServiceTests
    {
        private MandarinDbContext dbContext;
        private MandarinConfiguration mandarinConfiguration;

        [Test]
        public async Task GetArtistDetailsAsync_GivenInactiveArtistDataFromService_ReturnsEmptyList()
        {
            this.GivenMandarinConfigurationIsEmpty();
            this.GivenDbContextReturns(WellKnownTestData.Fruity.Stockist.InactiveArtist);
            var subject = new DatabaseArtistService(this.dbContext, Options.Create(this.mandarinConfiguration));
            var artistDetails = await subject.GetArtistsForDisplayAsync().ToList();
            Assert.That(artistDetails, Has.Count.Zero);
        }

        [Test]
        public async Task GetArtistDetailsAsync_GivenMinimalJsonDataFromService_ShouldDeserializeCorrectly()
        {
            this.GivenMandarinConfigurationIsEmpty();
            this.GivenDbContextReturns(WellKnownTestData.Fruity.Stockist.MinimalArtist);
            var subject = new DatabaseArtistService(this.dbContext, Options.Create(this.mandarinConfiguration));
            var artistDetails = await subject.GetArtistsForDisplayAsync().ToList();
            Assert.That(artistDetails, Has.Count.EqualTo(1));
            Assert.That(artistDetails[0].StockistName, Is.EqualTo("Artist Name"));
            Assert.That(artistDetails[0].Description, Is.EqualTo("Artist's Description."));
            Assert.That(artistDetails[0].Details.ImageUrl, Is.EqualTo(new Uri("https://localhost/static/images/artist1.jpg")));
            Assert.That(artistDetails[0].Details.TwitterHandle, Is.Null);
            Assert.That(artistDetails[0].Details.InstagramHandle, Is.Null);
            Assert.That(artistDetails[0].Details.FacebookHandle, Is.Null);
            Assert.That(artistDetails[0].Details.TumblrHandle, Is.Null);
            Assert.That(artistDetails[0].Details.WebsiteUrl, Is.Null);
        }

        [Test]
        public async Task GetArtistDetailsAsync_GivenJsonDataFromService_ShouldDeserializeCorrectly()
        {
            this.GivenMandarinConfigurationIsEmpty();
            this.GivenDbContextReturns(WellKnownTestData.Fruity.Stockist.FullArtist);
            var subject = new DatabaseArtistService(this.dbContext, Options.Create(this.mandarinConfiguration));
            var artistDetails = await subject.GetArtistsForDisplayAsync().ToList();
            Assert.That(artistDetails, Has.Count.EqualTo(1));
            Assert.That(artistDetails[0].StockistName, Is.EqualTo("Artist Name"));
            Assert.That(artistDetails[0].Description, Is.EqualTo("Artist's Description."));
            Assert.That(artistDetails[0].Details.ImageUrl, Is.EqualTo("https://localhost/static/images/artist1.jpg"));
            Assert.That(artistDetails[0].Details.TwitterHandle, Is.EqualTo("ArtistTwitter"));
            Assert.That(artistDetails[0].Details.InstagramHandle, Is.EqualTo("ArtistInstagram"));
            Assert.That(artistDetails[0].Details.FacebookHandle, Is.EqualTo("ArtistFacebook"));
            Assert.That(artistDetails[0].Details.TumblrHandle, Is.EqualTo("ArtistTumblr"));
            Assert.That(artistDetails[0].Details.WebsiteUrl, Is.EqualTo("https://localhost/artist/website"));
        }

        [Test]
        public async Task GetArtistDetailsForCommissionAsync_GivenConfigurationContainsAdditionalValues_ShouldContainAllValues()
        {
            var artist = TestData.Create<ArtistDetailsModel>();
            this.GivenMandarinConfigurationHasValue(artist);
            this.GivenDbContextReturns(WellKnownTestData.Fruity.Stockist.FullArtist);
            var subject = new DatabaseArtistService(this.dbContext, Options.Create(this.mandarinConfiguration));
            var artistDetails = await subject.GetArtistsForCommissionAsync().ToList();
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

        private void GivenDbContextReturns(Stockist artist)
        {
            var data = new List<Stockist> { artist }.AsQueryable();
            var mock = new Mock<DbSet<Stockist>>();
            mock.As<IQueryable<Stockist>>().Setup(m => m.Provider).Returns(data.Provider);
            mock.As<IQueryable<Stockist>>().Setup(m => m.Expression).Returns(data.Expression);
            mock.As<IQueryable<Stockist>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mock.As<IQueryable<Stockist>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            this.dbContext = Mock.Of<MandarinDbContext>(x => x.Stockist == mock.Object);
        }
    }
}
