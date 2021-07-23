using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using FluentAssertions.Execution;
using Mandarin.Commissions;
using Mandarin.Inventory;
using Mandarin.Services.Commission;
using Mandarin.Stockists;
using Mandarin.Tests.Data;
using Mandarin.Tests.Data.Extensions;
using Mandarin.Transactions;
using Moq;
using Xunit;

namespace Mandarin.Services.Tests.Commission
{
    public class CommissionServiceTests
    {
        private readonly Mock<ICommissionRepository> commissionRepository;
        private readonly Mock<IStockistService> stockistService;
        private readonly Mock<ITransactionService> transactionService;

        protected CommissionServiceTests()
        {
            this.commissionRepository = new Mock<ICommissionRepository>();
            this.stockistService = new Mock<IStockistService>();
            this.transactionService = new Mock<ITransactionService>();
        }

        private ICommissionService Subject =>
            new CommissionService(this.commissionRepository.Object,
                                  this.stockistService.Object,
                                  this.transactionService.Object);

        private void GivenTlmStockistExists()
        {
            var stockists = new List<Stockist> { WellKnownTestData.Stockists.TheLittleMandarin };
            this.stockistService.Setup(x => x.GetStockistsAsync()).ReturnsAsync(stockists);
        }

        private void GivenTransactionServiceReturnsData()
        {
            var product1 = TestData.Create<Product>().WithTlmProductCode() with { UnitPrice = 1.00m };
            var product2 = TestData.Create<Product>().WithTlmProductCode() with { UnitPrice = 5.00m };

            var transactions = new List<Transaction>
            {
                new()
                {
                    TransactionId = null,
                    TotalAmount = 10.00M,
                    Timestamp = DateTime.Now,
                    Subtransactions = new List<Subtransaction>
                    {
                        new()
                        {
                            Product = product1,
                            Quantity = 5,
                            Subtotal = 5.00m,
                        },
                        new()
                        {
                            Product = product2,
                            Quantity = 1,
                            Subtotal = 5.00m,
                        },
                    }.AsReadOnly(),
                },
                new()
                {
                    TransactionId = null,
                    TotalAmount = 50.00m,
                    Timestamp = DateTime.Now,
                    Subtransactions = new List<Subtransaction>
                    {
                        new()
                        {
                            Product = product2,
                            Quantity = 10,
                            Subtotal = 50.00m,
                        },
                    }.AsReadOnly(),
                },
            };

            this.transactionService.Setup(x => x.GetAllTransactions(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(transactions.ToObservable());
        }

        public class GetRecordOfSalesForPeriodAsyncTests : CommissionServiceTests
        {
            [Fact]
            public async Task ShouldCalculateCommissionCorrectly()
            {
                this.GivenTlmStockistExists();
                this.GivenTransactionServiceReturnsData();

                var actual = await this.Subject.GetRecordOfSalesForPeriodAsync(DateTime.Now, DateTime.Now);

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
