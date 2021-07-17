using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Commissions;
using Mandarin.Services.Stockists;
using Mandarin.Stockists;
using Mandarin.Tests.Data;
using Mandarin.Tests.Data.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Npgsql;
using Xunit;

namespace Mandarin.Services.Tests.Stockists
{
    public class StockistServiceTests
    {
        private readonly Mock<IStockistRepository> stockistRepository = new();
        private readonly Mock<ICommissionRepository> commissionRepository = new();

        private IStockistService Subject =>
            new StockistService(this.stockistRepository.Object,
                                this.commissionRepository.Object,
                                NullLogger<StockistService>.Instance);

        private void GivenRepositoriesThrowException()
        {
            this.stockistRepository.Setup(x => x.GetStockistByCode(It.IsAny<StockistCode>())).ThrowsAsync(new NpgsqlException("Not allowed."));
            this.stockistRepository.Setup(x => x.GetAllStockists()).ThrowsAsync(new NpgsqlException("Not allowed."));
            this.stockistRepository.Setup(x => x.SaveStockistAsync(It.IsAny<Stockist>())).ThrowsAsync(new NpgsqlException("Not allowed."));
        }

        private void GivenRepositoriesReturnWellKnownTestData()
        {
            this.stockistRepository.Setup(x => x.GetStockistByCode(WellKnownTestData.Stockists.KelbyTynan.StockistCode))
                .ReturnsAsync(WellKnownTestData.Stockists.KelbyTynan.WithoutCommission());
            this.stockistRepository.Setup(x => x.GetStockistByCode(WellKnownTestData.Stockists.OthilieMapples.StockistCode))
                .ReturnsAsync(WellKnownTestData.Stockists.OthilieMapples.WithoutCommission());
            this.stockistRepository.Setup(x => x.GetAllStockists())
                .ReturnsAsync(new List<Stockist>
                {
                    WellKnownTestData.Stockists.KelbyTynan.WithoutCommission(),
                    WellKnownTestData.Stockists.OthilieMapples.WithoutCommission(),
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
