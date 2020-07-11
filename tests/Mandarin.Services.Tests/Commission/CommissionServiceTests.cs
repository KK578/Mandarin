using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using AutoFixture;
using Bashi.Tests.Framework.Data;
using Mandarin.Models.Artists;
using Mandarin.Models.Inventory;
using Mandarin.Models.Transactions;
using Mandarin.Services.Commission;
using Mandarin.Services.Tests.Entity;
using Mandarin.Tests.Data;
using Mandarin.Tests.Data.Extensions;
using Moq;
using NUnit.Framework;

namespace Mandarin.Services.Tests.Commission
{
    [TestFixture]
    public class CommissionServiceTests
    {
        private Mock<IArtistService> artistService;
        private Mock<ITransactionService> transactionService;

        [Test]
        public async Task GetSalesByArtistForPeriod_ShouldCalculateCommissionCorrectly()
        {
            this.GivenArtistServiceReturnsData();
            this.GivenTransactionServiceReturnsData();

            var subject = new CommissionService(this.artistService.Object, this.transactionService.Object);
            var artistSales = await subject.GetSalesByArtistForPeriod(DateTime.Now, DateTime.Now).ToList().ToTask();

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

        private void GivenArtistServiceReturnsData()
        {
            this.artistService ??= new Mock<IArtistService>();
            this.artistService
                .Setup(x => x.GetArtistsForCommissionAsync())
                .Returns(new List<Stockist>
                {
                    MandarinFixture.Instance.Create<Stockist>().AsActive().WithTlmStockistCode().WithTenPercentCommission(),
                }.ToObservable());
        }

        private void GivenTransactionServiceReturnsData()
        {
            var product1 = TestData.Create<Product>().WithTlmProductCode().WithUnitPrice(1.00m);
            var product2 = TestData.Create<Product>().WithTlmProductCode().WithUnitPrice(5.00m);

            var transactions = new List<Transaction>
            {
                new Transaction(null, 10.00M, DateTime.Now, null, new List<Subtransaction>
                                {
                                    new Subtransaction(product1, 5, 5.00m),
                                    new Subtransaction(product2, 1, 5.00m),
                                }),
                new Transaction(null, 50.00m, DateTime.Now, null, new List<Subtransaction>
                                {
                                    new Subtransaction(product2, 10, 50.00m),
                                }),
            };
            this.transactionService ??= new Mock<ITransactionService>();
            this.transactionService.Setup(x => x.GetAllTransactions(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(transactions.ToObservable());
        }
    }
}
