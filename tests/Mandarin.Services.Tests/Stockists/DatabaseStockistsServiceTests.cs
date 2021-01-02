using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Database;
using Mandarin.Models.Stockists;
using Mandarin.Services.Stockists;
using Mandarin.Tests.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace Mandarin.Services.Tests.Stockists
{
    public class DatabaseStockistsServiceTests
    {
        private readonly Mock<MandarinDbContext> mandarinDbContext;

        protected DatabaseStockistsServiceTests()
        {
            this.mandarinDbContext = new Mock<MandarinDbContext>();
        }

        private IStockistService Subject => new DatabaseStockistService(this.mandarinDbContext.Object, NullLogger<DatabaseStockistService>.Instance);

        private void GivenStockistsExist(params Stockist[] stockists)
        {
            this.mandarinDbContext.Setup(x => x.Stockist).ReturnsDbSet(stockists);
        }

        public class GetStockistsAsyncTests : DatabaseStockistsServiceTests
        {
            [Fact]
            public async Task ShouldReturnAllStockistsInOrderOfStockistCode()
            {
                this.GivenStockistsExist(WellKnownTestData.Stockists.KelbyTynan,
                                         WellKnownTestData.Stockists.OthilieMapples);

                var artistDetails = await this.Subject.GetStockistsAsync();

                artistDetails.Should().HaveCount(2);
                artistDetails[0].Should().Be(WellKnownTestData.Stockists.OthilieMapples);
                artistDetails[1].Should().Be(WellKnownTestData.Stockists.KelbyTynan);
            }
        }
    }
}
