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
        private readonly IProductRepository productRepository;
        private readonly IFramePricesService framePricesService;
        private readonly IOptions<MandarinConfiguration> mandarinConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionMapper"/> class.
        /// </summary>
        /// <param name="productRepository">The application repository for interacting with products.</param>
        /// <param name="framePricesService">The application service for interacting with frame prices.</param>
        /// <param name="mandarinConfiguration">The application configuration.</param>
        public TransactionMapper(IProductRepository productRepository,
                                 IFramePricesService framePricesService,
                                 IOptions<MandarinConfiguration> mandarinConfiguration)
        {
            this.productRepository = productRepository;
            this.framePricesService = framePricesService;
            this.mandarinConfiguration = mandarinConfiguration;
        }

        /// <inheritdoc/>
        public IObservable<Transaction> MapToTransaction(Order order)
        {
            return this.CreateSubtransactions(order)
                       .Select(subtransactions => new Transaction
                       {
                           TransactionId = TransactionId.Of(order.Id),
                           TotalAmount = decimal.Divide(order.NetAmounts.TotalMoney?.Amount ?? 0, 100),
                           Timestamp = DateTime.Parse(order.CreatedAt),
                           Subtransactions = subtransactions.AsReadOnlyList(),
                       });
        }

        private static Subtransaction CreateSubtransactionFromMoney(Product product, Money money)
        {
            var quantity = money.Amount ?? 0;
            return new Subtransaction
            {
                Product = product,
                Quantity = (int)quantity,
                Subtotal = decimal.Divide(quantity, 100),
            };
        }

        private IObservable<IList<Subtransaction>> CreateSubtransactions(Order order)
        {
            var orderDate = DateTime.Parse(order.CreatedAt);
            var lineItems = order.LineItems.NullToEmpty().ToObservable().SelectMany(orderLineItem => this.CreateSubtransaction(orderLineItem, orderDate));
            var discounts = order.Discounts.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransactionFromDiscount);
            var returns = order.Returns.NullToEmpty().ToObservable().SelectMany(orderReturn => this.CreateSubtransactionsFromReturn(orderReturn, orderDate));
            var fees = order.ServiceCharges.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransactionFromFee);
            var tips = this.CreateSubtransactionFromTip(order.TotalTipMoney);
            return Observable.Merge(lineItems, discounts, returns, fees, tips).ToList();
        }

        private IObservable<Subtransaction> CreateSubtransaction(OrderLineItem orderLineItem, DateTime orderDate)
        {
            return this.GetProductAsync(ProductId.Of(orderLineItem.CatalogObjectId), ProductName.Of(orderLineItem.Name), orderDate)
                       .ToObservable()
                       .SelectMany(product =>
                       {
                           return this.framePricesService.GetFramePriceAsync(product.ProductCode, orderDate)
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

                    yield return new Subtransaction
                    {
                        Product = product,
                        Quantity = quantity,
                        Subtotal = subTotal,
                    };

                    yield return new Subtransaction
                    {
                        Product = new Product
                        {
                            ProductId = ProductId.Of("TLM-FRAMING"),
                            ProductCode = ProductCode.Of("TLM-FRAMING"),
                            ProductName = ProductName.Of("Commission for Frame"),
                            Description = $"Commission for Frame of {framePrice.ProductCode}",
                            UnitPrice = framePrice.Amount,
                        },
                        Quantity = quantity,
                        Subtotal = quantity * commissionSubtotal,
                    };
                }
                else
                {
                    var quantity = int.Parse(orderLineItem.Quantity);
                    var subTotal = quantity * decimal.Divide(orderLineItem.BasePriceMoney?.Amount ?? 0, 100);
                    yield return new Subtransaction
                    {
                        Product = product,
                        Quantity = quantity,
                        Subtotal = subTotal,
                    };
                }
            }
        }

        private IObservable<Subtransaction> CreateSubtransactionFromDiscount(OrderLineItemDiscount orderLineItemDiscount)
        {
            Product product;

            if (orderLineItemDiscount.Name.Contains("macaron", StringComparison.OrdinalIgnoreCase))
            {
                product = new Product
                {
                    ProductId = ProductId.Of("BUN-DCM"),
                    ProductCode = ProductCode.Of("BUN-DCM"),
                    ProductName = ProductName.Of("Box of Macarons Discount"),
                    Description = "Buy 6 macarons for \"£12.00\"",
                    UnitPrice = -0.01m,
                };
            }
            else if (orderLineItemDiscount.Name.Contains("pocky", StringComparison.OrdinalIgnoreCase))
            {
                product = new Product
                {
                    ProductId = ProductId.Of("BUN-DCP"),
                    ProductCode = ProductCode.Of("BUN-DCP"),
                    ProductName = ProductName.Of("Box of Pocky Discount"),
                    Description = "Discount on buying multiple packs of Pocky.",
                    UnitPrice = -0.01m,
                };
            }
            else
            {
                product = new Product
                {
                    ProductId = ProductId.Of("TLM-D"),
                    ProductCode = ProductCode.Of("TLM-D"),
                    ProductName = ProductName.Of("Other discounts"),
                    Description = "Discounts that aren't tracked.",
                    UnitPrice = -0.01m,
                };
            }

            var quantity = orderLineItemDiscount.AppliedMoney.Amount ?? 0;
            var amount = decimal.Divide(quantity, 100);
            return Observable.Return(new Subtransaction
            {
                Product = product,
                Quantity = (int)quantity,
                Subtotal = -amount,
            });
        }

        private IObservable<Subtransaction> CreateSubtransactionsFromReturn(OrderReturn orderReturn, DateTime orderDate)
        {
            return orderReturn.ReturnLineItems.ToObservable()
                              .SelectMany(async item =>
                              {
                                  var product = await this.GetProductAsync(ProductId.Of(item.CatalogObjectId), ProductName.Of(item.Name), orderDate);
                                  var quantity = -1 * int.Parse(item.Quantity);
                                  var subtotal = quantity * decimal.Divide(item.BasePriceMoney?.Amount ?? 0, 100);

                                  return new Subtransaction
                                  {
                                      Product = product,
                                      Quantity = quantity,
                                      Subtotal = subtotal,
                                  };
                              });
        }

        private IObservable<Subtransaction> CreateSubtransactionFromFee(OrderServiceCharge serviceCharge)
        {
            return Observable.FromAsync(() =>
                             {
                                 if (serviceCharge.Name?.Equals("Shipping", StringComparison.OrdinalIgnoreCase) == true)
                                 {
                                     return this.productRepository.GetProductAsync(ProductId.TlmDelivery);
                                 }

                                 return Task.FromResult(new Product
                                 {
                                     ProductId = ProductId.Of("TLM-FEES"),
                                     ProductCode = ProductCode.Of("TLM-" + serviceCharge.Name),
                                     ProductName = ProductName.Of(serviceCharge.Name),
                                     Description = "Unknown Fee.",
                                     UnitPrice = 0.01m,
                                 });
                             })
                             .Select(product => TransactionMapper.CreateSubtransactionFromMoney(product, serviceCharge.TotalMoney));
        }

        private IObservable<Subtransaction> CreateSubtransactionFromTip(Money tip)
        {
            if (tip?.Amount > 0)
            {
                return Observable.FromAsync(() => this.productRepository.GetProductAsync(ProductId.TlmTip))
                                 .Select(product => TransactionMapper.CreateSubtransactionFromMoney(product, tip));
            }

            return Observable.Empty<Subtransaction>();
        }

        private async Task<Product> GetProductAsync(ProductId squareId, ProductName name, DateTime orderDate)
        {
            if (squareId != null)
            {
                var product = await this.productRepository.GetProductAsync(squareId);
                return await MapProduct(product);
            }
            else if (name != null)
            {
                var product = await this.productRepository.GetProductAsync(name);
                return await MapProduct(product);
            }
            else
            {
                return new Product
                {
                    ProductId = null,
                    ProductCode = ProductCode.Of("TLM-Unknown"),
                    ProductName = ProductName.Of("Unknown Product"),
                    Description = "Unknown Product",
                    UnitPrice = null,
                };
            }

            Task<Product> MapProduct(Product originalProduct)
            {
                foreach (var mapping in this.mandarinConfiguration.Value.ProductMappings)
                {
                    if (orderDate > mapping.TransactionsAfterDate && mapping.Mappings.ContainsKey(originalProduct.ProductCode.Value))
                    {
                        var mappedProductCode = ProductCode.Of(mapping.Mappings[originalProduct.ProductCode.Value]);
                        return this.productRepository.GetProductAsync(mappedProductCode);
                    }
                }

                return Task.FromResult(originalProduct);
            }
        }
    }
}
