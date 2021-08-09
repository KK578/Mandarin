using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Mandarin.Commissions;
using Mandarin.Services.Commission;
using Mandarin.Stockists;
using Mandarin.Tests.Data;
using Mandarin.Transactions;
using Moq;
using NodaTime;
using Xunit;

namespace Mandarin.Services.Tests.Commission
{
    public class CommissionServiceTests
    {
        private static readonly Instant Start = Instant.FromUtc(2021, 06, 01, 00, 00, 00);
        private static readonly Instant End = Instant.FromUtc(2021, 07, 01, 00, 00, 00);

        private readonly Mock<IStockistService> stockistService;
        private readonly Mock<ITransactionRepository> transactionRepository;

        protected CommissionServiceTests()
        {
            this.stockistService = new Mock<IStockistService>();
            this.transactionRepository = new Mock<ITransactionRepository>();
        }

        private ICommissionService Subject =>
            new CommissionService(this.stockistService.Object, this.transactionRepository.Object);

        private void GivenTlmStockistExists()
        {
            var stockists = new List<Stockist> { WellKnownTestData.Stockists.TheLittleMandarin };
            this.stockistService.Setup(x => x.GetStockistsAsync()).ReturnsAsync(stockists);
        }

        private void GivenTransactionServiceReturnsData()
        {
            var product1 = MandarinFixture.Instance.NewProductTlm with { UnitPrice = 1.00m };
            var product2 = MandarinFixture.Instance.NewProductTlm with { UnitPrice = 5.00m };

            var transactions = new List<Transaction>
            {
                new()
                {
                    TransactionId = null,
                    ExternalTransactionId = null,
                    TotalAmount = 10.00M,
                    Timestamp = Instant.FromUtc(2021, 06, 15, 12, 00, 00),
                    Subtransactions = new List<Subtransaction>
                    {
                        new()
                        {
                            Product = product1,
                            Quantity = 5,
                            UnitPrice = 1.00M,
                        },
                        new()
                        {
                            Product = product2,
                            Quantity = 1,
                            UnitPrice = 5.00M,
                        },
                    }.AsReadOnly(),
                },
                new()
                {
                    TransactionId = null,
                    ExternalTransactionId = null,
                    TotalAmount = 50.00m,
                    Timestamp = Instant.FromUtc(2021, 06, 15, 12, 10, 00),
                    Subtransactions = new List<Subtransaction>
                    {
                        new()
                        {
                            Product = product2,
                            Quantity = 10,
                            UnitPrice = 5.00M,
                        },
                    }.AsReadOnly(),
                },
            };

            this.transactionRepository.Setup(x => x.GetAllTransactionsAsync(CommissionServiceTests.Start, CommissionServiceTests.End))
                .ReturnsAsync(transactions.AsReadOnly());
        }

        public class GetRecordOfSalesForPeriodAsyncTests : CommissionServiceTests
        {
            [Fact]
            public async Task ShouldCalculateCommissionCorrectly()
            {
                this.GivenTlmStockistExists();
                this.GivenTransactionServiceReturnsData();

                var actual = await this.Subject.GetRecordOfSalesForPeriodAsync(CommissionServiceTests.Start, CommissionServiceTests.End);

                actual.Should().HaveCount(1);
                using (new AssertionScope())
                {
                    actual[0].Subtotal.Should().Be(60.00m);
                    actual[0].CommissionTotal.Should().Be(-6.00m);
                    actual[0].Total.Should().Be(54.00m);
                    actual[0].Sales.Should().HaveCount(2);
                }

                using (new AssertionScope())
                {
                    actual[0].Sales[0].Quantity.Should().Be(5);
                    actual[0].Sales[0].Subtotal.Should().Be(5);
                    actual[0].Sales[0].Commission.Should().Be(-0.5m);
                    actual[0].Sales[0].Total.Should().Be(4.5m);
                    actual[0].Sales[1].Quantity.Should().Be(11);
                    actual[0].Sales[1].Subtotal.Should().Be(55m);
                    actual[0].Sales[1].Commission.Should().Be(-5.5m);
                    actual[0].Sales[1].Total.Should().Be(49.5m);
                }
            }
        }
    }
}
