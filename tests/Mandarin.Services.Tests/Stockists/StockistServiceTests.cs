using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Commissions;
using Mandarin.Services.Stockists;
using Mandarin.Stockists;
using Mandarin.Tests.Data;
using Moq;
using Npgsql;
using Xunit;

namespace Mandarin.Services.Tests.Stockists
{
    public class StockistServiceTests
    {
        private readonly Mock<IStockistRepository> stockistRepository = new();
        private readonly Mock<ICommissionRepository> commissionRepository = new();

        private IStockistService Subject => new StockistService(this.stockistRepository.Object, this.commissionRepository.Object);

        private void GivenRepositoriesThrowException()
        {
            this.stockistRepository.Setup(x => x.GetStockistAsync(It.IsAny<StockistCode>())).ThrowsAsync(new NpgsqlException("Not allowed."));
            this.stockistRepository.Setup(x => x.GetAllStockistsAsync()).ThrowsAsync(new NpgsqlException("Not allowed."));
            this.stockistRepository.Setup(x => x.SaveStockistAsync(It.IsAny<Stockist>())).ThrowsAsync(new NpgsqlException("Not allowed."));
        }

        private void GivenRepositoriesReturnWellKnownTestData()
        {
            var kelbyTynanWithoutCommission = WellKnownTestData.Stockists.KelbyTynan with { Commission = null };
            var othilieMapplesWithoutCommission = WellKnownTestData.Stockists.OthilieMapples with { Commission = null };

            this.stockistRepository.Setup(x => x.GetStockistAsync(WellKnownTestData.Stockists.KelbyTynan.StockistCode))
                .ReturnsAsync(kelbyTynanWithoutCommission);
            this.stockistRepository.Setup(x => x.GetStockistAsync(WellKnownTestData.Stockists.OthilieMapples.StockistCode))
                .ReturnsAsync(othilieMapplesWithoutCommission);
            this.stockistRepository.Setup(x => x.GetAllStockistsAsync())
                .ReturnsAsync(new List<Stockist>
                {
                    kelbyTynanWithoutCommission,
                    othilieMapplesWithoutCommission,
                });

            this.commissionRepository.Setup(x => x.GetCommissionByStockist(WellKnownTestData.Stockists.KelbyTynan.StockistId))
                .ReturnsAsync(WellKnownTestData.Stockists.KelbyTynan.Commission);
            this.commissionRepository.Setup(x => x.GetCommissionByStockist(WellKnownTestData.Stockists.OthilieMapples.StockistId))
                .ReturnsAsync(WellKnownTestData.Stockists.OthilieMapples.Commission);
        }

        public class GetStockistByCodeAsyncTests : StockistServiceTests
        {
            [Fact]
            public async Task ShouldThrowIfRepositoryThrows()
            {
                this.GivenRepositoriesThrowException();
                await this.Subject.Invoking(x => x.GetStockistByCodeAsync(WellKnownTestData.Stockists.KelbyTynan.StockistCode))
                          .Should()
                          .ThrowAsync<NpgsqlException>();
            }

            [Fact]
            public async Task ShouldGetCompleteStockistIncludingCommission()
            {
                this.GivenRepositoriesReturnWellKnownTestData();
                var actual = await this.Subject.GetStockistByCodeAsync(WellKnownTestData.Stockists.KelbyTynan.StockistCode);
                actual.Should().BeEquivalentTo(WellKnownTestData.Stockists.KelbyTynan);
            }
        }

        public class GetAllStockistsAsyncTests : StockistServiceTests
        {
            [Fact]
            public async Task ShouldThrowIfRepositoryThrows()
            {
                this.GivenRepositoriesThrowException();
                await this.Subject.Invoking(x => x.GetStockistsAsync()).Should().ThrowAsync<NpgsqlException>();
            }

            [Fact]
            public async Task ShouldGetCompleteStockistsIncludingCommission()
            {
                this.GivenRepositoriesReturnWellKnownTestData();
                var actual = await this.Subject.GetStockistsAsync();
                actual.Should().BeEquivalentTo(WellKnownTestData.Stockists.KelbyTynan, WellKnownTestData.Stockists.OthilieMapples);
            }
        }

        public class SaveStockistsAsyncTests : StockistServiceTests
        {
            [Fact]
            public async Task ShouldThrowIfRepositoryThrows()
            {
                this.GivenRepositoriesThrowException();
                await this.Subject.Invoking(x => x.SaveStockistAsync(WellKnownTestData.Stockists.ArlueneWoodes))
                          .Should()
                          .ThrowAsync<NpgsqlException>();
            }
        }
    }
}
