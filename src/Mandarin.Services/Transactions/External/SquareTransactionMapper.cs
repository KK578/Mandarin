﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Bashi.Core.Extensions;
using Mandarin.Configuration;
using Mandarin.Inventory;
using Mandarin.Transactions;
using Mandarin.Transactions.External;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Text;
using Square.Models;
using Transaction = Mandarin.Transactions.Transaction;

namespace Mandarin.Services.Transactions.External
{
    /// <inheritdoc />
    internal sealed class SquareTransactionMapper : ISquareTransactionMapper
    {
        private readonly IProductRepository productRepository;
        private readonly IFramePricesService framePricesService;
        private readonly IOptions<MandarinConfiguration> mandarinConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareTransactionMapper"/> class.
        /// </summary>
        /// <param name="productRepository">The application repository for interacting with products.</param>
        /// <param name="framePricesService">The application service for interacting with frame prices.</param>
        /// <param name="mandarinConfiguration">The application configuration.</param>
        public SquareTransactionMapper(IProductRepository productRepository,
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
                           ExternalTransactionId = ExternalTransactionId.Of(order.Id),
                           TotalAmount = decimal.Divide(order.NetAmounts.TotalMoney?.Amount ?? 0, 100),
                           Timestamp = InstantPattern.General.Parse(order.CreatedAt).GetValueOrThrow(),
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
                UnitPrice = 0.01M,
            };
        }

        private IObservable<IList<Subtransaction>> CreateSubtransactions(Order order)
        {
            var orderDate = InstantPattern.General.Parse(order.CreatedAt).GetValueOrThrow();
            var lineItems = order.LineItems.NullToEmpty().ToObservable().SelectMany(orderLineItem => this.CreateSubtransaction(orderLineItem, orderDate));
            var discounts = order.Discounts.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransactionFromDiscount);
            var returns = order.Returns.NullToEmpty().ToObservable().SelectMany(orderReturn => this.CreateSubtransactionsFromReturn(orderReturn, orderDate));
            var fees = order.ServiceCharges.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransactionFromFee);
            var tips = this.CreateSubtransactionFromTip(order.TotalTipMoney);
            return Observable.Merge(lineItems, discounts, returns, fees, tips).ToList();
        }

        private IObservable<Subtransaction> CreateSubtransaction(OrderLineItem orderLineItem, Instant orderDate)
        {
            return this.GetProductAsync(ProductId.Of(orderLineItem.CatalogObjectId), ProductName.Of(orderLineItem.Name), orderDate)
                       .ToObservable()
                       .SelectMany(product =>
                       {
                           return this.framePricesService.GetFramePriceAsync(product.ProductCode, orderDate)
                                      .ToObservable()
                                      .SelectMany(framePrice => Create(product, framePrice));
                       });

            IObservable<Subtransaction> Create(Product product, FramePrice framePrice)
            {
                var quantity = int.Parse(orderLineItem.Quantity);

                if (framePrice != null)
                {
                    var commissionSubtotal = framePrice.Amount;

                    return Observable.Create<Subtransaction>(async o =>
                    {
                        var framing = await this.productRepository.GetProductAsync(ProductId.TlmFraming);

                        o.OnNext(new Subtransaction
                        {
                            Product = product,
                            Quantity = quantity,
                            UnitPrice = orderLineItem.BasePriceMoney.ToDecimal() - commissionSubtotal,
                        });

                        o.OnNext(new Subtransaction
                        {
                            Product = framing,
                            Quantity = quantity,
                            UnitPrice = commissionSubtotal,
                        });
                    });
                }
                else
                {
                    return Observable.Return(new Subtransaction
                    {
                        Product = product,
                        Quantity = quantity,
                        UnitPrice = orderLineItem.BasePriceMoney.ToDecimal(),
                    });
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
            return Observable.Return(new Subtransaction
            {
                Product = product,
                Quantity = (int)quantity,
                UnitPrice = -0.01M,
            });
        }

        private IObservable<Subtransaction> CreateSubtransactionsFromReturn(OrderReturn orderReturn, Instant orderDate)
        {
            return orderReturn.ReturnLineItems.ToObservable()
                              .SelectMany(async item =>
                              {
                                  var product = await this.GetProductAsync(ProductId.Of(item.CatalogObjectId), ProductName.Of(item.Name), orderDate);
                                  var quantity = -1 * int.Parse(item.Quantity);

                                  return new Subtransaction
                                  {
                                      Product = product,
                                      Quantity = quantity,
                                      UnitPrice = item.BasePriceMoney.ToDecimal(),
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

                                 return this.productRepository.GetProductAsync(ProductId.TlmUnknown);
                             })
                             .Select(product => SquareTransactionMapper.CreateSubtransactionFromMoney(product, serviceCharge.TotalMoney));
        }

        private IObservable<Subtransaction> CreateSubtransactionFromTip(Money tip)
        {
            if (tip?.Amount > 0)
            {
                return Observable.FromAsync(() => this.productRepository.GetProductAsync(ProductId.TlmTip))
                                 .Select(product => SquareTransactionMapper.CreateSubtransactionFromMoney(product, tip));
            }

            return Observable.Empty<Subtransaction>();
        }

        private async Task<Product> GetProductAsync(ProductId productId, ProductName name, Instant orderDate)
        {
            if (productId != null)
            {
                var product = await this.productRepository.GetProductAsync(productId);
                return await MapProduct(product);
            }
            else if (name != null)
            {
                var product = await this.productRepository.GetProductAsync(name);
                return await MapProduct(product);
            }
            else
            {
                return await this.productRepository.GetProductAsync(ProductId.TlmUnknown);
            }

            Task<Product> MapProduct(Product originalProduct)
            {
                var orderDateTime = orderDate.ToDateTimeUtc();
                foreach (var mapping in this.mandarinConfiguration.Value.ProductMappings)
                {
                    if (orderDateTime > mapping.TransactionsAfterDate && mapping.Mappings.ContainsKey(originalProduct.ProductCode.Value))
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
