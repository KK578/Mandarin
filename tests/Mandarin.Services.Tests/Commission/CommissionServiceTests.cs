using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using AutoFixture;
using Bashi.Tests.Framework.Data;
using Mandarin.Database;
using Mandarin.Models.Commissions;
using Mandarin.Models.Common;
using Mandarin.Models.Inventory;
using Mandarin.Models.Stockists;
using Mandarin.Models.Transactions;
using Mandarin.Services.Commission;
using Mandarin.Tests.Data;
using Mandarin.Tests.Data.Extensions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Commission
{
    [TestFixture]
    public class CommissionServiceTests
    {
        private Mock<IStockistService> stockistService;
        private Mock<ITransactionService> transactionService;
        private Mock<MandarinDbContext> mandarinDbContext;

        private ICommissionService Subject =>
            new CommissionService(this.stockistService.Object,
                                  this.transactionService.Object,
                                  this.mandarinDbContext.Object);

        [SetUp]
        public void SetUp()
        {
            this.stockistService = new Mock<IStockistService>();
            this.transactionService = new Mock<ITransactionService>();
            this.mandarinDbContext = new Mock<MandarinDbContext>();
        }

        private void GivenCommissionRateGroups(params int[] rates)
        {
            var data = rates.Select((rate, i) => new CommissionRateGroup { GroupId = i, Rate = rate });
            this.mandarinDbContext.Setup(x => x.CommissionRateGroup).ReturnsDbSet(data);
        }

        private void GivenTlmStockistExists()
        {
            this.stockistService.Setup(x => x.GetStockistsAsync())
                                  .Returns(new List<Stockist>
                                  {
                                      MandarinFixture.Instance.Create<Stockist>()
                                                     .WithStatus(StatusMode.Active)
                                                     .WithTlmStockistCode()
                                                     .WithTenPercentCommission(),
                                  }.ToObservable());
        }

        private void GivenTransactionServiceReturnsData()
        {
            var product1 = TestData.Create<Product>().WithTlmProductCode().WithUnitPrice(1.00m);
            var product2 = TestData.Create<Product>().WithTlmProductCode().WithUnitPrice(5.00m);

            var transactions = new List<Transaction>
            {
                new(null, 10.00M, DateTime.Now, null, new List<Subtransaction>
                    {
                        new(product1, 5, 5.00m),
                        new(product2, 1, 5.00m),
                    }),
                new(null, 50.00m, DateTime.Now, null, new List<Subtransaction>
                    {
                        new(product2, 10, 50.00m),
                    }),
            };

            this.transactionService.Setup(x => x.GetAllTransactions(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(transactions.ToObservable());
        }

        [TestFixture]
        private class GetCommissionRateGroupsTests : CommissionServiceTests
        {
            [Test]
            public async Task ShouldReturnAllEntries()
            {
                this.GivenCommissionRateGroups(10, 20, 30);
                var actual = await this.Subject.GetCommissionRateGroupsAsync();
                Assert.That(actual, Has.Count.EqualTo(3));
            }

            [Test]
            public async Task ShouldReturnEntriesInAscendingOrderByRate()
            {
                this.GivenCommissionRateGroups(40, 20, 10, 50, 30, 100);
                var actual = await this.Subject.GetCommissionRateGroupsAsync();
                Assert.That(actual.Select(x => x.Rate), Is.EqualTo(new[] { 10, 20, 30, 40, 50, 100 }).AsCollection);
            }
        }

        [TestFixture]
        private class GetRecordOfSalesAsyncTests : CommissionServiceTests
        {
            [Test]
            public async Task ShouldCalculateCommissionCorrectly()
            {
                this.GivenTlmStockistExists();
                this.GivenTransactionServiceReturnsData();

                var artistSales = await this.Subject.GetRecordOfSalesAsync(DateTime.Now, DateTime.Now);

                Assert.That(artistSales.Count, Is.EqualTo(1));
                Assert.That(artistSales[0].Subtotal, Is.EqualTo(60.00m));
                Assert.That(artistSales[0].CommissionTotal, Is.EqualTo(-6.00m));
                Assert.That(artistSales[0].Total, Is.EqualTo(54.00m));
                Assert.That(artistSales[0].Sales.Count, Is.EqualTo(2));
                Assert.That(artistSales[0].Sales[0].Quantity, Is.EqualTo(5));
                Assert.That(artistSales[0].Sales[0].Subtotal, Is.EqualTo(5));
                Assert.That(artistSales[0].Sales[0].Commission, Is.EqualTo(-0.5m));
                Assert.That(artistSales[0].Sales[0].Total, Is.EqualTo(4.5m));
                Assert.That(artistSales[0].Sales[1].Quantity, Is.EqualTo(11));
                Assert.That(artistSales[0].Sales[1].Subtotal, Is.EqualTo(55m));
                Assert.That(artistSales[0].Sales[1].Commission, Is.EqualTo(-5.5m));
                Assert.That(artistSales[0].Sales[1].Total, Is.EqualTo(49.5m));
            }
        }
    }
}
