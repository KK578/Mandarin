using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Inventory;
using Mandarin.Services.Transactions.External;
using Mandarin.Tests.Data;
using Moq;
using NodaTime;
using Square;
using Xunit;
using Product = Mandarin.Inventory.Product;

namespace Mandarin.Services.Tests.Transactions.External
{
    public class SquareTransactionMapperTests
    {
        private const string OrderDateString = "2021-08-01T14:12:13Z";
        private static readonly Instant OrderDate = Instant.FromUtc(2021, 08, 01, 14, 12, 13);

        private readonly Mock<IProductRepository> productRepository;
        private readonly Mock<IFramePricesService> framePricesService;

        protected SquareTransactionMapperTests()
        {
            this.productRepository = new Mock<IProductRepository>();
            this.framePricesService = new Mock<IFramePricesService>();
        }

        private ISquareTransactionMapper Subject => new SquareTransactionMapper(this.productRepository.Object, this.framePricesService.Object);


        private void GivenInventoryServiceSetUpWithProduct(Product product)
        {
            this.productRepository.Setup(x => x.GetProductAsync(product.ProductId)).ReturnsAsync(product);
            this.productRepository.Setup(x => x.GetProductAsync(product.ProductCode)).ReturnsAsync(product);
            this.productRepository.Setup(x => x.GetProductAsync(product.ProductName)).ReturnsAsync(product);
        }

        private void GivenFramePriceExists(Product product, FramePrice framePrice)
        {
            this.framePricesService.Setup(x => x.GetFramePriceAsync(product.ProductCode, SquareTransactionMapperTests.OrderDate)).ReturnsAsync(framePrice);
            this.productRepository.Setup(x => x.GetProductAsync(ProductId.TlmFraming)).ReturnsAsync(WellKnownTestData.Products.TlmFraming);
        }

        private Order GivenOrderProductAsLineItem(Product product)
        {
            var lineItems = new List<OrderLineItem>
            {
                new()
                {
                    Uid = "2",
                    CatalogObjectId = product.ProductId.Value,
                    Name = product.ProductName.Value,
                    BasePriceMoney = new Money { Amount = 500, Currency = Currency.Gbp },
                    TotalMoney = new Money { Amount = 1000, Currency = Currency.Gbp },
                    Quantity = "1",
                },
            };

            return new Order
            {
                Id = MandarinFixture.Instance.NewString,
                LocationId = "Location",
                LineItems = lineItems,
                NetAmounts = new OrderMoneyAmounts { TotalMoney = new Money { Amount = 1000, Currency = Currency.Gbp } },
                CreatedAt = SquareTransactionMapperTests.OrderDateString,
            };
        }

        private Order GivenOrderProductWithDiscount(Product product)
        {
            var lineItems = new List<OrderLineItem>
            {
                new()
                {
                    Uid = "2",
                    CatalogObjectId = product.ProductId.Value,
                    Name = product.ProductName.Value,
                    BasePriceMoney = new Money { Amount = 5000, Currency = Currency.Gbp },
                    TotalMoney = new Money { Amount = 10000, Currency = Currency.Gbp },
                    Quantity = "1",
                },
            };
            var discounts = new List<OrderLineItemDiscount>
            {
                new()
                {
                    CatalogObjectId = product.ProductId.Value,
                    Name = product.ProductName.Value,
                    AmountMoney = new Money { Amount = 2000, Currency = Currency.Gbp },
                    AppliedMoney = new Money { Amount = 2000, Currency = Currency.Gbp },
                },
            };
            return new Order
            {
                Id = MandarinFixture.Instance.NewString,
                LocationId = "Location",
                LineItems = lineItems,
                Discounts = discounts,
                NetAmounts = new OrderMoneyAmounts { TotalMoney = new Money { Amount = 8000, Currency = Currency.Gbp } },
                CreatedAt = SquareTransactionMapperTests.OrderDateString,
            };
        }

        private Order GivenOrderProductAsReturn(Product product)
        {
            var returns = new List<OrderReturnLineItem>
            {
                new()
                {
                    Uid = "3",
                    CatalogObjectId = product.ProductId.Value,
                    Name = product.ProductName.Value,
                    BasePriceMoney = new Money { Amount = 500, Currency = Currency.Gbp },
                    TotalMoney = new Money { Amount = 1500, Currency = Currency.Gbp },
                    Quantity = "1",
                },
            };
            return new Order
            {
                Id = MandarinFixture.Instance.NewString,
                LocationId = "Location",
                Returns = new List<OrderReturn> { new() { ReturnLineItems = returns } },
                NetAmounts = new OrderMoneyAmounts { TotalMoney = new Money { Amount = -1500, Currency = Currency.Gbp } },
                CreatedAt = SquareTransactionMapperTests.OrderDateString,
            };
        }

        private Order GivenOrderServiceChargeReturn()
        {
            var returns = new List<OrderReturnServiceCharge>
            {
                new()
                {
                    Uid = "4",
                    Name = "Shipping",
                    TotalMoney = new Money { Amount = 500, Currency = Currency.Gbp },
                },
            };

            return new Order
            {
                Id = MandarinFixture.Instance.NewString,
                LocationId = "Location",
                Returns = new List<OrderReturn> { new() { ReturnServiceCharges = returns } },
                NetAmounts = new OrderMoneyAmounts { TotalMoney = new Money { Amount = -500, Currency = Currency.Gbp } },
                CreatedAt = SquareTransactionMapperTests.OrderDateString,
            };
        }

        public class MapToTransactionTests : SquareTransactionMapperTests
        {
            [Fact]
            public async Task ShouldConvertLineItemsToATransaction()
            {
                var product = MandarinFixture.Instance.NewProduct;
                this.GivenInventoryServiceSetUpWithProduct(product);
                var order = this.GivenOrderProductAsLineItem(product);
                var transactions = await this.Subject.MapToTransaction(order).ToList().ToTask();

                transactions.Should().HaveCount(1);
                transactions[0].Timestamp.Should().Be(SquareTransactionMapperTests.OrderDate);
                transactions[0].TotalAmount.Should().Be(10.00m);
                transactions[0].Subtransactions[0].Product.Should().Be(product);
                transactions[0].Subtransactions[0].Quantity.Should().Be(2);
                transactions[0].Subtransactions[0].UnitPrice.Should().Be(5.00m);
                transactions[0].Subtransactions[0].Subtotal.Should().Be(10.00m);
            }

            [Fact]
            public async Task ShouldIncludeTheFramePriceAsATransaction()
            {
                var product = MandarinFixture.Instance.NewProduct;
                var framePrice = new FramePrice
                {
                    ProductCode = product.ProductCode,
                    Amount = 1.00m,
                };
                this.GivenInventoryServiceSetUpWithProduct(product);
                this.GivenFramePriceExists(product, framePrice);
                var order = this.GivenOrderProductAsLineItem(product);
                var transactions = await this.Subject.MapToTransaction(order).ToList().ToTask();

                transactions.Should().HaveCount(1);
                transactions[0].TotalAmount.Should().Be(10.00m);
                transactions[0].Subtransactions.Should().HaveCount(2);
                transactions[0].Subtransactions[0].Product.Should().Be(product);
                transactions[0].Subtransactions[0].Quantity.Should().Be(2);
                transactions[0].Subtransactions[0].UnitPrice.Should().Be(4.00m);
                transactions[0].Subtransactions[0].Subtotal.Should().Be(8.00m);
                transactions[0].Subtransactions[1].Product.ProductCode.Value.Should().StartWith("TLM");
                transactions[0].Subtransactions[1].Quantity.Should().Be(2);
                transactions[0].Subtransactions[1].UnitPrice.Should().Be(1.00m);
                transactions[0].Subtransactions[1].Subtotal.Should().Be(2.00m);
            }

            [Fact]
            public async Task ShouldMapDiscountsToATransaction()
            {
                var product = MandarinFixture.Instance.NewProduct;
                this.GivenInventoryServiceSetUpWithProduct(product);
                var order = this.GivenOrderProductWithDiscount(product);
                var transactions = await this.Subject.MapToTransaction(order).ToList().ToTask();

                transactions.Should().HaveCount(1);
                transactions[0].TotalAmount.Should().Be(80.00m);
                transactions[0].Subtransactions[0].Quantity.Should().Be(2);
                transactions[0].Subtransactions[0].UnitPrice.Should().Be(50.00m);
                transactions[0].Subtransactions[0].Subtotal.Should().Be(100.00M);
                transactions[0].Subtransactions[1].Quantity.Should().Be(2000);
                transactions[0].Subtransactions[1].UnitPrice.Should().Be(-0.01m);
                transactions[0].Subtransactions[1].Subtotal.Should().Be(-20.00m);
            }

            [Fact]
            public async Task ShouldConvertReturnsToTransactions()
            {
                var product = MandarinFixture.Instance.NewProduct;
                this.GivenInventoryServiceSetUpWithProduct(product);
                var order = this.GivenOrderProductAsReturn(product);
                var transactions = await this.Subject.MapToTransaction(order).ToList().ToTask();

                transactions.Should().HaveCount(1);
                transactions[0].TotalAmount.Should().Be(-15.00m);
                transactions[0].Subtransactions[0].Quantity.Should().Be(-3);
                transactions[0].Subtransactions[0].UnitPrice.Should().Be(5.00m);
                transactions[0].Subtransactions[0].Subtotal.Should().Be(-15.00m);
            }


            [Fact]
            public async Task ShouldConvertServiceChargeReturnsToTransactions()
            {
                this.GivenInventoryServiceSetUpWithProduct(WellKnownTestData.Products.TlmDelivery);
                var order = this.GivenOrderServiceChargeReturn();
                var transactions = await this.Subject.MapToTransaction(order).ToList().ToTask();

                transactions.Should().HaveCount(1);
                transactions[0].TotalAmount.Should().Be(-5.00m);
                transactions[0].Subtransactions[0].Product.Should().Be(WellKnownTestData.Products.TlmDelivery);
                transactions[0].Subtransactions[0].Quantity.Should().Be(-500);
                transactions[0].Subtransactions[0].UnitPrice.Should().Be(0.01m);
                transactions[0].Subtransactions[0].Subtotal.Should().Be(-5.00m);
            }

            [Fact]
            public async Task ShouldIncludeDeliveryFeesAsAnItem()
            {
                this.GivenInventoryServiceSetUpWithProduct(WellKnownTestData.Products.TheTrickster);
                var order = new Order()
                {
                    LocationId = "Location",
                    LineItems = new List<OrderLineItem>
                    {
                        new()
                        {
                            Uid = "1",
                            CatalogObjectId = "CatalogId",
                            Name = "[HC20W-003] The Trickster",
                            BasePriceMoney = new Money { Amount = 1100, Currency = Currency.Gbp },
                            TotalMoney = new Money { Amount = 1100, Currency = Currency.Gbp },
                            Quantity = "1",
                        },
                    },
                    ServiceCharges = new List<OrderServiceCharge>
                    {
                        new()
                        {
                            Name = "Shipping",
                            AmountMoney = new Money { Amount = 500, Currency = Currency.Gbp },
                            TotalMoney = new Money { Amount = 500, Currency = Currency.Gbp },
                        },
                    },
                    NetAmounts = new OrderMoneyAmounts { TotalMoney = new Money { Amount = 1600, Currency = Currency.Gbp } },
                    CreatedAt = SquareTransactionMapperTests.OrderDateString,
                };

                var transactions = await this.Subject.MapToTransaction(order).ToList().ToTask();
                transactions.Should().HaveCount(1);
                transactions[0].TotalAmount.Should().Be(16.00m);
                transactions[0].Subtransactions.Should().HaveCount(2);
                transactions[0].Subtransactions[0].Quantity.Should().Be(1);
                transactions[0].Subtransactions[0].UnitPrice.Should().Be(11.00m);
                transactions[0].Subtransactions[0].Subtotal.Should().Be(11.00m);
                transactions[0].Subtransactions[1].Quantity.Should().Be(500);
                transactions[0].Subtransactions[1].UnitPrice.Should().Be(0.01m);
                transactions[0].Subtransactions[1].Subtotal.Should().Be(5.00m);
            }
        }
    }
}
