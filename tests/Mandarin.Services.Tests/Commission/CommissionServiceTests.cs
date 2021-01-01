﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using FluentAssertions.Execution;
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
using Xunit;

namespace Mandarin.Services.Tests.Commission
{
    public class CommissionServiceTests
    {
        private readonly Mock<IStockistService> stockistService;
        private readonly Mock<ITransactionService> transactionService;
        private readonly Mock<MandarinDbContext> mandarinDbContext;

        protected CommissionServiceTests()
        {
            this.stockistService = new Mock<IStockistService>();
            this.transactionService = new Mock<ITransactionService>();
            this.mandarinDbContext = new Mock<MandarinDbContext>();
        }

        private ICommissionService Subject =>
            new CommissionService(this.stockistService.Object,
                                  this.transactionService.Object,
                                  this.mandarinDbContext.Object);

        private void GivenCommissionRateGroups(params int[] rates)
        {
            var data = rates.Select((rate, i) => new CommissionRateGroup { GroupId = i, Rate = rate });
            this.mandarinDbContext.Setup(x => x.CommissionRateGroup).ReturnsDbSet(data);
        }

        private void GivenTlmStockistExists()
        {
            var stockists = new List<Stockist>
            {
                MandarinFixture.Instance.Create<Stockist>()
                               .WithStatus(StatusMode.Active)
                               .WithTlmStockistCode()
                               .WithTenPercentCommission(),
            };
            this.stockistService.Setup(x => x.GetStockistsAsync()).ReturnsAsync(stockists);
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

        public class GetCommissionRateGroupsTests : CommissionServiceTests
        {
            [Fact]
            public async Task ShouldReturnAllEntries()
            {
                this.GivenCommissionRateGroups(10, 20, 30);
                var actual = await this.Subject.GetCommissionRateGroupsAsync();
                actual.Should().HaveCount(3);
            }

            [Fact]
            public async Task ShouldReturnEntriesInAscendingOrderByRate()
            {
                this.GivenCommissionRateGroups(40, 20, 10, 50, 30, 100);
                var actual = await this.Subject.GetCommissionRateGroupsAsync();
                actual.Select(x => x.Rate).Should().HaveCount(6).And.BeInAscendingOrder();
            }
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
