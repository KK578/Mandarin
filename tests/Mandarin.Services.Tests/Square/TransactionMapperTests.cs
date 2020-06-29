using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Mandarin.Models.Commissions;
using Mandarin.Models.Inventory;
using Mandarin.Services.Square;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Square.Models;
using Transaction = Mandarin.Models.Transactions.Transaction;

namespace Mandarin.Services.Tests.Square
{
    [TestFixture]
    public class TransactionMapperTests
    {
        private Mock<IQueryableInventoryService> inventoryService;

        [Test]
        public async Task MapToTransaction_GivenOrderWithLineItems_ShouldMapCorrectly()
        {
            var product = TestData.Create<Product>();
            this.GivenInventoryServiceSetUpWithProduct(product);
            var order = this.GivenOrderProductAsLineItem(product);
            var transactions = await this.WhenMappingTransactions(order);
            Assert.That(transactions.Count, Is.EqualTo(1));
            Assert.That(transactions[0].TotalAmount, Is.EqualTo(10.00m));
            Assert.That(transactions[0].Subtransactions[0].Product, Is.EqualTo(product));
            Assert.That(transactions[0].Subtransactions[0].Quantity, Is.EqualTo(2));
            Assert.That(transactions[0].Subtransactions[0].TransactionUnitPrice, Is.EqualTo(5.00m));
            Assert.That(transactions[0].Subtransactions[0].Subtotal, Is.EqualTo(10.00m));
        }

        [Test]
        public async Task MapToTransaction_GivenOrderWithFixedCommission_ShouldMapCorrectly()
        {
            var product = TestData.Create<Product>();
            var fixedCommission = new FixedCommissionAmount(product.ProductCode, 1.00m);
            this.GivenInventoryServiceSetUpWithProduct(product);
            this.GivenInventoryServiceSetUpWithFixedCommission(product, fixedCommission);
            var order = this.GivenOrderProductAsLineItem(product);
            var transactions = await this.WhenMappingTransactions(order);
            Assert.That(transactions.Count, Is.EqualTo(1));
            Assert.That(transactions[0].TotalAmount, Is.EqualTo(10.00m));
            Assert.That(transactions[0].Subtransactions.Count, Is.EqualTo(2));
            Assert.That(transactions[0].Subtransactions[0].Product, Is.EqualTo(product));
            Assert.That(transactions[0].Subtransactions[0].Quantity, Is.EqualTo(2));
            Assert.That(transactions[0].Subtransactions[0].TransactionUnitPrice, Is.EqualTo(4.00m));
            Assert.That(transactions[0].Subtransactions[0].Subtotal, Is.EqualTo(8.00m));
            Assert.That(transactions[0].Subtransactions[1].Product.ProductCode, Does.StartWith("TLM"));
            Assert.That(transactions[0].Subtransactions[1].Quantity, Is.EqualTo(2));
            Assert.That(transactions[0].Subtransactions[1].TransactionUnitPrice, Is.EqualTo(1.00m));
            Assert.That(transactions[0].Subtransactions[1].Subtotal, Is.EqualTo(2.00m));
        }

        [Test]
        public async Task MapToTransaction_GivenOrderWithDiscount_ShouldMapCorrectly()
        {
            var product = TestData.Create<Product>();
            this.GivenInventoryServiceSetUpWithProduct(product);
            var order = this.GivenOrderProductAsDiscount(product);
            var transactions = await this.WhenMappingTransactions(order);
            Assert.That(transactions.Count, Is.EqualTo(1));
            Assert.That(transactions[0].TotalAmount, Is.EqualTo(-20.00m));
            Assert.That(transactions[0].Subtransactions[0].Quantity, Is.EqualTo(2000));
            Assert.That(transactions[0].Subtransactions[0].TransactionUnitPrice, Is.EqualTo(-0.01m));
            Assert.That(transactions[0].Subtransactions[0].Subtotal, Is.EqualTo(-20.00m));
        }

        [Test]
        public async Task MapToTransaction_GivenOrderWithReturn_ShouldMapCorrectly()
        {
            var product = TestData.Create<Product>();
            this.GivenInventoryServiceSetUpWithProduct(product);
            var order = this.GivenOrderProductAsReturn(product);
            var transactions = await this.WhenMappingTransactions(order);
            Assert.That(transactions.Count, Is.EqualTo(1));
            Assert.That(transactions[0].TotalAmount, Is.EqualTo(-15.00m));
            Assert.That(transactions[0].Subtransactions[0].Quantity, Is.EqualTo(-3));
            Assert.That(transactions[0].Subtransactions[0].TransactionUnitPrice, Is.EqualTo(5.00m));
            Assert.That(transactions[0].Subtransactions[0].Subtotal, Is.EqualTo(-15.00m));
        }

        private void GivenInventoryServiceSetUpWithProduct(Product product)
        {
            this.inventoryService ??= new Mock<IQueryableInventoryService>();
            this.inventoryService.Setup(x => x.GetProductBySquareIdAsync(product.SquareId)).ReturnsAsync(product);
            this.inventoryService.Setup(x => x.GetProductByNameAsync(product.ProductName)).ReturnsAsync(product);
        }

        private void GivenInventoryServiceSetUpWithFixedCommission(Product product, FixedCommissionAmount fixedCommissionAmount)
        {
            this.inventoryService ??= new Mock<IQueryableInventoryService>();
            this.inventoryService.Setup(x => x.GetFixedCommissionAmount(product)).ReturnsAsync(fixedCommissionAmount);
        }


        private Order GivenOrderProductAsLineItem(Product product)
        {
            var lineItems = new List<OrderLineItem>
            {
                new OrderLineItem("2",
                                  catalogObjectId: product.SquareId,
                                  name: product.ProductName,
                                  totalMoney: new Money(1000, "GBP")),
            };
            return new Order("Location",
                             TestData.WellKnownString,
                             lineItems: lineItems,
                             totalMoney: new Money(1000, "GBP"),
                             createdAt: DateTime.Now.ToString("O"));
        }

        private Order GivenOrderProductAsDiscount(Product product)
        {
            var discounts = new List<OrderLineItemDiscount>
            {
                new OrderLineItemDiscount(catalogObjectId: product.SquareId,
                                          name: product.ProductName,
                                          amountMoney: new Money(2000, "GBP")),
            };
            return new Order("Location",
                             TestData.WellKnownString,
                             discounts: discounts,
                             totalMoney: new Money(-2000, "GBP"),
                             createdAt: DateTime.Now.ToString("O"));
        }

        private Order GivenOrderProductAsReturn(Product product)
        {
            var returns = new List<OrderReturnLineItem>
            {
                new OrderReturnLineItem("3",
                                        catalogObjectId: product.SquareId,
                                        name: product.ProductName,
                                        totalMoney: new Money(-1500, "GBP")),
            };
            return new Order("Location",
                             TestData.WellKnownString,
                             returns: new List<OrderReturn> { new OrderReturn(returnLineItems: returns) },
                             totalMoney: new Money(-1500, "GBP"),
                             createdAt: DateTime.Now.ToString("O"));
        }

        private Task<IList<Transaction>> WhenMappingTransactions(Order order)
        {
            var subject = new TransactionMapper(this.inventoryService.Object);
            return subject.MapToTransaction(order).ToList().ToTask();
        }
    }
}
