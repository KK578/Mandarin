using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Models.Artists;
using Mandarin.Services.Artists;
using Mandarin.Tests.Data;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Entity
{
    [TestFixture]
    public class DatabaseArtistServiceTests
    {
        private Mock<MandarinDbContext> mandarinDbContext;

        private IArtistService Subject => new DatabaseArtistService(this.mandarinDbContext.Object);

        [SetUp]
        public void SetUp()
        {
            this.mandarinDbContext = new Mock<MandarinDbContext>();
        }

        private void GivenDbContextReturns(params Stockist[] artists)
        {
            this.mandarinDbContext.Setup(x => x.Stockist).ReturnsDbSet(artists);
        }

        [TestFixture]
        private class GetArtistsForCommissionAsync : DatabaseArtistServiceTests
        {
            [Test]
            public async Task ShouldReturnAllArtistsInOrderOfStockistCode()
            {
                this.GivenDbContextReturns(WellKnownTestData.Stockists.InactiveArtist,
                                           WellKnownTestData.Stockists.HiddenArtist,
                                           WellKnownTestData.Stockists.MinimalArtist);

                var artistDetails = await this.Subject.GetArtistsForCommissionAsync().ToList();
                Assert.That(artistDetails, Has.Exactly(3).Items);
                Assert.That(artistDetails[0], Is.EqualTo(WellKnownTestData.Stockists.HiddenArtist));
                Assert.That(artistDetails[1], Is.EqualTo(WellKnownTestData.Stockists.InactiveArtist));
                Assert.That(artistDetails[2], Is.EqualTo(WellKnownTestData.Stockists.MinimalArtist));
            }
        }
    }
}
