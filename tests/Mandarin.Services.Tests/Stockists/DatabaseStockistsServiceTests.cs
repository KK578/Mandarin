using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Models.Stockists;
using Mandarin.Services.Stockists;
using Mandarin.Tests.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Stockists
{
    [TestFixture]
    public class DatabaseStockistsServiceTests
    {
        private Mock<MandarinDbContext> mandarinDbContext;

        private IStockistService Subject => new EntityFrameworkStockistService(this.mandarinDbContext.Object, NullLogger<EntityFrameworkStockistService>.Instance);

        [SetUp]
        public void SetUp()
        {
            this.mandarinDbContext = new Mock<MandarinDbContext>();
        }

        private void GivenStockistsExist(params Stockist[] stockists)
        {
            this.mandarinDbContext.Setup(x => x.Stockist).ReturnsDbSet(stockists);
        }

        [TestFixture]
        private class GetStockistsAsyncTests : DatabaseStockistsServiceTests
        {
            [Test]
            public async Task ShouldReturnAllStockistsInOrderOfStockistCode()
            {
                this.GivenStockistsExist(WellKnownTestData.Stockists.InactiveStockist,
                                         WellKnownTestData.Stockists.HiddenStockist,
                                         WellKnownTestData.Stockists.MinimalStockist);

                var artistDetails = await this.Subject.GetStockistsAsync();
                Assert.That(artistDetails, Has.Exactly(3).Items);
                Assert.That(artistDetails[0], Is.EqualTo(WellKnownTestData.Stockists.HiddenStockist));
                Assert.That(artistDetails[1], Is.EqualTo(WellKnownTestData.Stockists.InactiveStockist));
                Assert.That(artistDetails[2], Is.EqualTo(WellKnownTestData.Stockists.MinimalStockist));
            }
        }
    }
}
