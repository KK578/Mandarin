using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Configuration;
using Mandarin.Inventory;
using Mandarin.Services.Transactions;
using Microsoft.Extensions.Options;
using Moq;
using Square.Models;
using Xunit;

namespace Mandarin.Services.Tests.Transactions
{
    public class TransactionMapperTests
    {
        private readonly DateTime orderDate = DateTime.Now;

        private readonly Mock<IQueryableProductService> productService;
        private readonly MandarinConfiguration configuration;
        private readonly Mock<IFramePricesService> framePricesService;

        public TransactionMapperTests()
        {
            this.productService = new Mock<IQueryableProductService>();
            this.configuration = new MandarinConfiguration();
            this.framePricesService = new Mock<IFramePricesService>();
        }

        private ITransactionMapper Subject =>
            new TransactionMapper(this.productService.Object,
                                  this.framePricesService.Object,
                                  Options.Create(this.configuration));


        private void GivenInventoryServiceSetUpWithProduct(Product product)
        {
            this.productService.Setup(x => x.GetProductBySquareIdAsync(product.SquareId)).ReturnsAsync(product);
            this.productService.Setup(x => x.GetProductByProductCodeAsync(product.ProductCode)).ReturnsAsync(product);
            this.productService.Setup(x => x.GetProductByNameAsync(product.ProductName)).ReturnsAsync(product);
        }

        private void GivenFramePriceExists(Product product, FramePrice framePrice)
        {
            this.framePricesService.Setup(x => x.GetFramePriceAsync(product.ProductCode)).ReturnsAsync(framePrice);
        }

        private void GivenConfigurationWithMappings(Product product, Product mappedProduct)
        {
            this.configuration.ProductMappings.Add(new ProductMapping
            {
                TransactionsAfterDate = this.orderDate.AddDays(-1),
                Mappings = new Dictionary<string, string> { { product.ProductCode, mappedProduct.ProductCode } },
            });
            this.GivenInventoryServiceSetUpWithProduct(mappedProduct);
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

        public class MapToTransactionTests : TransactionMapperTests
        {
            [Fact]
            public async Task ShouldConvertLineItemsToATransaction()
            {
                var product = TestData.Create<Product>();
                this.GivenInventoryServiceSetUpWithProduct(product);
                var order = this.GivenOrderProductAsLineItem(product);
                var transactions = await this.Subject.MapToTransaction(order).ToList().ToTask();

                transactions.Should().HaveCount(1);
                transactions[0].TotalAmount.Should().Be(10.00m);
                transactions[0].Subtransactions[0].Product.Should().Be(product);
                transactions[0].Subtransactions[0].Quantity.Should().Be(2);
                transactions[0].Subtransactions[0].TransactionUnitPrice.Should().Be(5.00m);
                transactions[0].Subtransactions[0].Subtotal.Should().Be(10.00m);
            }

            [Fact]
            public async Task ShouldPrioritiseProductMappingsIfApplicationToAProduct()
            {
                var product = TestData.Create<Product>();
                var mappedProduct = TestData.Create<Product>();
                this.GivenInventoryServiceSetUpWithProduct(product);
                this.GivenConfigurationWithMappings(product, mappedProduct);
                var order = this.GivenOrderProductAsLineItem(product);
                var transactions = await this.Subject.MapToTransaction(order).ToList().ToTask();

                transactions.Should().HaveCount(1);
                transactions[0].Subtransactions[0].Product.Should().Be(mappedProduct);
            }

            [Fact]
            public async Task ShouldIncludeTheFramePriceAsATransaction()
            {
                var product = TestData.Create<Product>();
                var framePrice = new FramePrice(product.ProductCode, 1.00m);
                this.GivenInventoryServiceSetUpWithProduct(product);
                this.GivenFramePriceExists(product, framePrice);
                var order = this.GivenOrderProductAsLineItem(product);
                var transactions = await this.Subject.MapToTransaction(order).ToList().ToTask();

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
            public async Task ShouldMapDiscountsToATransaction()
            {
                var product = TestData.Create<Product>();
                this.GivenInventoryServiceSetUpWithProduct(product);
                var order = this.GivenOrderProductAsDiscount(product);
                var transactions = await this.Subject.MapToTransaction(order).ToList().ToTask();

                transactions.Should().HaveCount(1);
                transactions[0].TotalAmount.Should().Be(-20.00m);
                transactions[0].Subtransactions[0].Quantity.Should().Be(2000);
                transactions[0].Subtransactions[0].TransactionUnitPrice.Should().Be(-0.01m);
                transactions[0].Subtransactions[0].Subtotal.Should().Be(-20.00m);
            }

            [Fact]
            public async Task ShouldConvertReturnsToTransactions()
            {
                var product = TestData.Create<Product>();
                this.GivenInventoryServiceSetUpWithProduct(product);
                var order = this.GivenOrderProductAsReturn(product);
                var transactions = await this.Subject.MapToTransaction(order).ToList().ToTask();

                transactions.Should().HaveCount(1);
                transactions[0].TotalAmount.Should().Be(-15.00m);
                transactions[0].Subtransactions[0].Quantity.Should().Be(-3);
                transactions[0].Subtransactions[0].TransactionUnitPrice.Should().Be(5.00m);
                transactions[0].Subtransactions[0].Subtotal.Should().Be(-15.00m);
            }

            [Fact]
            public async Task ShouldIncludeDeliveryFeesAsAnItem()
            {
                var product = new Product("CatalogId", "HC20W-003", "The Trickster", TestData.NextString(), 11.00m);
                this.GivenInventoryServiceSetUpWithProduct(product);
                var order = new Order.Builder("Location")
                            .LineItems(new List<OrderLineItem>
                            {
                                new OrderLineItem.Builder("1")
                                    .CatalogObjectId("CatalogId")
                                    .Name("[HC20W-003] The Trickster")
                                    .BasePriceMoney(new Money(1100, "GBP"))
                                    .TotalMoney(new Money(1100, "GBP"))
                                    .Build(),
                            })
                            .ServiceCharges(new List<OrderServiceCharge>
                            {
                                new OrderServiceCharge.Builder()
                                    .Name("Shipping")
                                    .AmountMoney(new Money(500, "GBP"))
                                    .TotalMoney(new Money(500, "GBP"))
                                    .Build(),
                            })
                            .CreatedAt(this.orderDate.ToString("O"))
                            .TotalMoney(new Money(1600, "GBP"))
                            .Build();

                var transactions = await this.Subject.MapToTransaction(order).ToList().ToTask();
                transactions.Should().HaveCount(1);
                transactions[0].TotalAmount.Should().Be(16.00m);
                transactions[0].Subtransactions.Should().HaveCount(2);
                transactions[0].Subtransactions[0].Quantity.Should().Be(1);
                transactions[0].Subtransactions[0].TransactionUnitPrice.Should().Be(11.00m);
                transactions[0].Subtransactions[0].Subtotal.Should().Be(11.00m);
                transactions[0].Subtransactions[1].Quantity.Should().Be(500);
                transactions[0].Subtransactions[1].TransactionUnitPrice.Should().Be(0.01m);
                transactions[0].Subtransactions[1].Subtotal.Should().Be(5.00m);
            }
        }
    }
}
