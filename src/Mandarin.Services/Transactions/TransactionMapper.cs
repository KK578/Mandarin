using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Bashi.Core.Extensions;
using Mandarin.Configuration;
using Mandarin.Inventory;
using Mandarin.Transactions;
using Microsoft.Extensions.Options;
using Square.Models;
using Transaction = Mandarin.Transactions.Transaction;

namespace Mandarin.Services.Transactions
{
    /// <inheritdoc />
    internal sealed class TransactionMapper : ITransactionMapper
    {
        private readonly IQueryableProductService productService;
        private readonly IFramePricesService framePricesService;
        private readonly IOptions<MandarinConfiguration> mandarinConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionMapper"/> class.
        /// </summary>
        /// <param name="productService">The application service for interacting with products.</param>
        /// <param name="framePricesService">The application service for interacting with frame prices.</param>
        /// <param name="mandarinConfiguration">The application configuration.</param>
        public TransactionMapper(IQueryableProductService productService,
                                 IFramePricesService framePricesService,
                                 IOptions<MandarinConfiguration> mandarinConfiguration)
        {
            this.productService = productService;
            this.framePricesService = framePricesService;
            this.mandarinConfiguration = mandarinConfiguration;
        }

        /// <inheritdoc/>
        public IObservable<Transaction> MapToTransaction(Order order)
        {
            return this.CreateSubtransactions(order)
                       .Select(subtransactions =>
                       {
                           var transactionId = order.Id;
                           var totalAmount = decimal.Divide(order.TotalMoney?.Amount ?? 0, 100);
                           var timestamp = DateTime.Parse(order.CreatedAt);
                           var insertedBy = order.Source?.Name;
                           return new Transaction(transactionId, totalAmount, timestamp, insertedBy, subtransactions);
                       });
        }

        private IObservable<IList<Subtransaction>> CreateSubtransactions(Order order)
        {
            var orderDate = DateTime.Parse(order.CreatedAt);
            var lineItems = order.LineItems.NullToEmpty().ToObservable().SelectMany(orderLineItem => this.CreateSubtransaction(orderLineItem, orderDate));
            var discounts = order.Discounts.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransactionFromDiscount);
            var returns = order.Returns.NullToEmpty().ToObservable().SelectMany(orderReturn => this.CreateSubtransactionsFromReturn(orderReturn, orderDate));
            var fees = order.ServiceCharges.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransactionFromFee);
            return Observable.Merge(lineItems, discounts, returns, fees).ToList();
        }

        private IObservable<Subtransaction> CreateSubtransaction(OrderLineItem orderLineItem, DateTime orderDate)
        {
            return this.GetProductAsync(orderLineItem.CatalogObjectId, orderLineItem.Name, orderDate)
                       .ToObservable()
                       .SelectMany(product =>
                       {
                           return this.framePricesService.GetFramePriceAsync(product.ProductCode)
                                      .ToObservable()
                                      .SelectMany(framePrice => Create(product, framePrice));
                       });

            IEnumerable<Subtransaction> Create(Product product, FramePrice framePrice)
            {
                if (framePrice != null)
                {
                    var quantity = int.Parse(orderLineItem.Quantity);
                    var commissionSubtotal = framePrice.Amount;
                    var subTotal = quantity * (decimal.Divide(orderLineItem.BasePriceMoney?.Amount ?? 0, 100) - commissionSubtotal);

                    yield return new Subtransaction(product, quantity, subTotal);
                    yield return new Subtransaction(new Product("TLM-" + framePrice.ProductCode,
                                                                "TLM-" + framePrice.ProductCode,
                                                                $"Frame for {framePrice.ProductCode}",
                                                                null,
                                                                framePrice.Amount),
                                                    quantity,
                                                    quantity * commissionSubtotal);
                }
                else
                {
                    var quantity = int.Parse(orderLineItem.Quantity);
                    var subTotal = quantity * decimal.Divide(orderLineItem.BasePriceMoney?.Amount ?? 0, 100);
                    yield return new Subtransaction(product, quantity, subTotal);
                }
            }
        }

        private IObservable<Subtransaction> CreateSubtransactionFromDiscount(OrderLineItemDiscount orderLineItemDiscount)
        {
            Product product;

            if (orderLineItemDiscount.Name.Contains("macaron", StringComparison.OrdinalIgnoreCase))
            {
                product = new Product("BUN-DCM", "BUN-DCM", "Box of Macarons Discount", "Buy 6 macarons for \"£12.00\"", -0.01m);
            }
            else if (orderLineItemDiscount.Name.Contains("pocky", StringComparison.OrdinalIgnoreCase))
            {
                product = new Product("BUN-DCP", "BUN-DCP", "Box of Pocky Discount", "Discount on buying multiple packs of Pocky.", -0.01m);
            }
            else
            {
                product = new Product("TLM-D", "TLM-D", "Other discounts", "Discounts that aren't tracked.", -0.01m);
            }

            var quantity = orderLineItemDiscount.AppliedMoney.Amount ?? 0;
            var amount = decimal.Divide(quantity, 100);
            var subtransaction = new Subtransaction(product, (int)quantity, -amount);
            return Observable.Return(subtransaction);
        }

        private IObservable<Subtransaction> CreateSubtransactionsFromReturn(OrderReturn orderReturn, DateTime orderDate)
        {
            return orderReturn.ReturnLineItems.ToObservable()
                              .SelectMany(async item =>
                              {
                                  var product = await this.GetProductAsync(item.CatalogObjectId, item.Name, orderDate);
                                  var quantity = -1 * int.Parse(item.Quantity);
                                  var subtotal = quantity * decimal.Divide(item.BasePriceMoney?.Amount ?? 0, 100);
                                  return new Subtransaction(product, quantity, subtotal);
                              });
        }

        private IObservable<Subtransaction> CreateSubtransactionFromFee(OrderServiceCharge serviceCharge)
        {
            Product product;
            if (serviceCharge.Name?.Equals("Shipping", StringComparison.OrdinalIgnoreCase) == true)
            {
                product = new Product("TLM-DELIVERY", "TLM-DELIVERY", "Shipping Fees", "Delivery costs charged to customers.", 0.01m);
            }
            else
            {
                product = new Product("TLM-FEES", "TLM-" + serviceCharge.Name, serviceCharge.Name, "Unknown Fee.", 0.01m);
            }

            var quantity = serviceCharge.TotalMoney.Amount ?? 0;
            var amount = decimal.Divide(quantity, 100);
            var transaction = new Subtransaction(product, (int)quantity, amount);
            return Observable.Return(transaction);
        }

        private async Task<Product> GetProductAsync(string squareId, string name, DateTime orderDate)
        {
            if (squareId != null)
            {
                var product = await this.productService.GetProductBySquareIdAsync(squareId);
                return await MapProduct(product);
            }
            else if (name != null)
            {
                var product = await this.productService.GetProductByNameAsync(name);
                return await MapProduct(product);
            }
            else
            {
                var unknownProduct = new Product(null, "TLM-Unknown", "Unknown Product", "Unknown Product", null);
                return unknownProduct;
            }

            Task<Product> MapProduct(Product originalProduct)
            {
                foreach (var mapping in this.mandarinConfiguration.Value.ProductMappings)
                {
                    if (orderDate > mapping.TransactionsAfterDate && mapping.Mappings.ContainsKey(originalProduct.ProductCode))
                    {
                        return this.productService.GetProductByProductCodeAsync(mapping.Mappings[originalProduct.ProductCode]);
                    }
                }

                return Task.FromResult(originalProduct);
            }
        }
    }
}
