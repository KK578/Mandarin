using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Models.Artists;
using Mandarin.Services.Artists;
using Mandarin.Tests.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Entity
{
    [TestFixture]
    public class DatabaseArtistServiceTests
    {
        private MandarinDbContext dbContext;

        [Test]
        public async Task GetArtistDetailsAsync_GivenArtists_ShouldOnlyContainActiveArtists()
        {
            this.GivenDbContextReturns(WellKnownTestData.Stockists.InactiveArtist,
                                       WellKnownTestData.Stockists.HiddenArtist,
                                       WellKnownTestData.Stockists.MinimalArtist);
            var subject = new DatabaseArtistService(this.dbContext);
            var actual = await subject.GetArtistsForDisplayAsync().ToList();

            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual, Is.EqualTo(new[] { WellKnownTestData.Stockists.MinimalArtist }).AsCollection);
        }

        [Test]
        public async Task GetArtistDetailsForCommissionAsync_GivenArtists_ShouldReturnAllArtistsInOrderOfStockistCode()
        {
            this.GivenDbContextReturns(WellKnownTestData.Stockists.InactiveArtist,
                                       WellKnownTestData.Stockists.HiddenArtist,
                                       WellKnownTestData.Stockists.MinimalArtist);

            var subject = new DatabaseArtistService(this.dbContext);
            var artistDetails = await subject.GetArtistsForCommissionAsync().ToList();
            Assert.That(artistDetails, Has.Exactly(3).Items);
            Assert.That(artistDetails[0], Is.EqualTo(WellKnownTestData.Stockists.HiddenArtist));
            Assert.That(artistDetails[1], Is.EqualTo(WellKnownTestData.Stockists.InactiveArtist));
            Assert.That(artistDetails[2], Is.EqualTo(WellKnownTestData.Stockists.MinimalArtist));
        }

        private IQueryable<Stockist> GivenDbContextReturns(params Stockist[] artists)
        {
            var data = artists.AsQueryable();
            var mock = new Mock<DbSet<Stockist>>();
            mock.As<IQueryable<Stockist>>().Setup(m => m.Provider).Returns(data.Provider);
            mock.As<IQueryable<Stockist>>().Setup(m => m.Expression).Returns(data.Expression);
            mock.As<IQueryable<Stockist>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            this.dbContext = Mock.Of<MandarinDbContext>(x => x.Stockist == mock.Object);
            return data;
        }
    }
}
