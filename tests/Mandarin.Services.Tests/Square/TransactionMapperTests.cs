using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Commissions;
using Mandarin.Configuration;
using Mandarin.Inventory;
using Mandarin.Services.Square;
using Microsoft.Extensions.Options;
using Moq;
using Square.Models;
using Xunit;
using Transaction = Mandarin.Transactions.Transaction;

namespace Mandarin.Services.Tests.Square
{
    public class TransactionMapperTests
    {
        private readonly DateTime orderDate = DateTime.Now;

        private Mock<IQueryableInventoryService> inventoryService;
        private MandarinConfiguration configuration;

        [Fact]
        public async Task MapToTransaction_GivenOrderWithLineItems_ShouldMapCorrectly()
        {
            var product = TestData.Create<Product>();
            this.GivenInventoryServiceSetUpWithProduct(product);
            this.GivenConfigurationWithNoMappings();
            var order = this.GivenOrderProductAsLineItem(product);
            var transactions = await this.WhenMappingTransactions(order);

            transactions.Should().HaveCount(1);
            transactions[0].TotalAmount.Should().Be(10.00m);
            transactions[0].Subtransactions[0].Product.Should().Be(product);
            transactions[0].Subtransactions[0].Quantity.Should().Be(2);
            transactions[0].Subtransactions[0].TransactionUnitPrice.Should().Be(5.00m);
            transactions[0].Subtransactions[0].Subtotal.Should().Be(10.00m);
        }

        [Fact]
        public async Task MapToTransaction_GivenOrderWithLineItems_WhenConfigurationMapsProduct_ShouldMapCorrectly()
        {
            var product = TestData.Create<Product>();
            var mappedProduct = TestData.Create<Product>();
            this.GivenInventoryServiceSetUpWithProduct(product);
            this.GivenConfigurationWithMappings(product, mappedProduct);
            var order = this.GivenOrderProductAsLineItem(product);
            var transactions = await this.WhenMappingTransactions(order);

            transactions.Should().HaveCount(1);
            transactions[0].Subtransactions[0].Product.Should().Be(mappedProduct);
        }

        [Fact]
        public async Task MapToTransaction_GivenOrderWithFixedCommission_ShouldMapCorrectly()
        {
            var product = TestData.Create<Product>();
            var fixedCommission = new FixedCommissionAmount(product.ProductCode, 1.00m);
            this.GivenInventoryServiceSetUpWithProduct(product);
            this.GivenInventoryServiceSetUpWithFixedCommission(product, fixedCommission);
            this.GivenConfigurationWithNoMappings();
            var order = this.GivenOrderProductAsLineItem(product);
            var transactions = await this.WhenMappingTransactions(order);

            transactions.Should().HaveCount(1);
            transactions[0].TotalAmount.Should().Be(10.00m);
            transactions[0].Subtransactions.Should().HaveCount(2);
            transactions[0].Subtransactions[0].Product.Should().Be(product);
            transactions[0].Subtransactions[0].Quantity.Should().Be(2);
            transactions[0].Subtransactions[0].TransactionUnitPrice.Should().Be(4.00m);
            transactions[0].Subtransactions[0].Subtotal.Should().Be(8.00m);
            transactions[0].Subtransactions[1].Product.ProductCode.Should().StartWith("TLM");
            transactions[0].Subtransactions[1].Quantity.Should().Be(2);
            transactions[0].Subtransactions[1].TransactionUnitPrice.Should().Be(1.00m);
            transactions[0].Subtransactions[1].Subtotal.Should().Be(2.00m);
        }

        [Fact]
        public async Task MapToTransaction_GivenOrderWithDiscount_ShouldMapCorrectly()
        {
            var product = TestData.Create<Product>();
            this.GivenInventoryServiceSetUpWithProduct(product);
            this.GivenConfigurationWithNoMappings();
            var order = this.GivenOrderProductAsDiscount(product);
            var transactions = await this.WhenMappingTransactions(order);

            transactions.Should().HaveCount(1);
            transactions[0].TotalAmount.Should().Be(-20.00m);
            transactions[0].Subtransactions[0].Quantity.Should().Be(2000);
            transactions[0].Subtransactions[0].TransactionUnitPrice.Should().Be(-0.01m);
            transactions[0].Subtransactions[0].Subtotal.Should().Be(-20.00m);
        }

        [Fact]
        public async Task MapToTransaction_GivenOrderWithReturn_ShouldMapCorrectly()
        {
            var product = TestData.Create<Product>();
            this.GivenInventoryServiceSetUpWithProduct(product);
            this.GivenConfigurationWithNoMappings();
            var order = this.GivenOrderProductAsReturn(product);
            var transactions = await this.WhenMappingTransactions(order);

            transactions.Should().HaveCount(1);
            transactions[0].TotalAmount.Should().Be(-15.00m);
            transactions[0].Subtransactions[0].Quantity.Should().Be(-3);
            transactions[0].Subtransactions[0].TransactionUnitPrice.Should().Be(5.00m);
            transactions[0].Subtransactions[0].Subtotal.Should().Be(-15.00m);
        }

        private void GivenInventoryServiceSetUpWithProduct(Product product)
        {
            this.inventoryService ??= new Mock<IQueryableInventoryService>();
            this.inventoryService.Setup(x => x.GetProductBySquareIdAsync(product.SquareId)).ReturnsAsync(product);
            this.inventoryService.Setup(x => x.GetProductByProductCodeAsync(product.ProductCode)).ReturnsAsync(product);
            this.inventoryService.Setup(x => x.GetProductByNameAsync(product.ProductName)).ReturnsAsync(product);
        }

        private void GivenInventoryServiceSetUpWithFixedCommission(Product product, FixedCommissionAmount fixedCommissionAmount)
        {
            this.inventoryService ??= new Mock<IQueryableInventoryService>();
            this.inventoryService.Setup(x => x.GetFixedCommissionAmount(product)).ReturnsAsync(fixedCommissionAmount);
        }


        private void GivenConfigurationWithNoMappings()
        {
            this.configuration = new MandarinConfiguration();
        }

        private void GivenConfigurationWithMappings(Product product, Product mappedProduct)
        {
            this.configuration = new MandarinConfiguration();
            this.configuration.ProductMappings = new List<ProductMapping>
            {
                new()
                {
                    TransactionsAfterDate = this.orderDate.AddDays(-1),
                    Mappings = new Dictionary<string, string>
                    {
                        { product.ProductCode, mappedProduct.ProductCode },
                    },
                },
            };

            this.inventoryService.Setup(x => x.GetProductBySquareIdAsync(mappedProduct.SquareId)).ReturnsAsync(mappedProduct);
            this.inventoryService.Setup(x => x.GetProductByProductCodeAsync(mappedProduct.ProductCode)).ReturnsAsync(mappedProduct);
            this.inventoryService.Setup(x => x.GetProductByNameAsync(mappedProduct.ProductName)).ReturnsAsync(mappedProduct);
        }




        private Order GivenOrderProductAsLineItem(Product product)
        {
            var lineItems = new List<OrderLineItem>
            {
                new("2",
                    catalogObjectId: product.SquareId,
                    name: product.ProductName,
                    basePriceMoney: new Money(500, "GBP"),
                    totalMoney: new Money(1000, "GBP")),
            };
            return new Order("Location",
                             TestData.WellKnownString,
                             lineItems: lineItems,
                             totalMoney: new Money(1000, "GBP"),
                             createdAt: this.orderDate.ToString("O"));
        }

        private Order GivenOrderProductAsDiscount(Product product)
        {
            var discounts = new List<OrderLineItemDiscount>
            {
                new(catalogObjectId: product.SquareId,
                    name: product.ProductName,
                    amountMoney: new Money(2000, "GBP"),
                    appliedMoney: new Money(2000, "GBP")),
            };
            return new Order("Location",
                             TestData.WellKnownString,
                             discounts: discounts,
                             totalMoney: new Money(-2000, "GBP"),
                             createdAt: this.orderDate.ToString("O"));
        }

        private Order GivenOrderProductAsReturn(Product product)
        {
            var returns = new List<OrderReturnLineItem>
            {
                new("3",
                    catalogObjectId: product.SquareId,
                    name: product.ProductName,
                    basePriceMoney: new Money(500, "GBP"),
                    totalMoney: new Money(-1500, "GBP")),
            };
            return new Order("Location",
                             TestData.WellKnownString,
                             returns: new List<OrderReturn> { new(returnLineItems: returns) },
                             totalMoney: new Money(-1500, "GBP"),
                             createdAt: this.orderDate.ToString("O"));
        }

        private Task<IList<Transaction>> WhenMappingTransactions(Order order)
        {
            var subject = new TransactionMapper(this.inventoryService.Object, Options.Create(this.configuration));
            return subject.MapToTransaction(order).ToList().ToTask();
        }
    }
}
