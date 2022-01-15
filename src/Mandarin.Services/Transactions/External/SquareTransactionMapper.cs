using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Bashi.Core.Extensions;
using Mandarin.Inventory;
using Mandarin.Transactions;
using Mandarin.Transactions.External;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareTransactionMapper"/> class.
        /// </summary>
        /// <param name="productRepository">The application repository for interacting with products.</param>
        /// <param name="framePricesService">The application service for interacting with frame prices.</param>
        public SquareTransactionMapper(IProductRepository productRepository, IFramePricesService framePricesService)
        {
            this.productRepository = productRepository;
            this.framePricesService = framePricesService;
        }

        /// <inheritdoc/>
        public IObservable<Transaction> MapToTransaction(Order order)
        {
            return this.CreateSubtransactions(order)
                       .Select(subtransactions => new Transaction
                       {
                           ExternalTransactionId = ExternalTransactionId.Of(order.Id),
                           TotalAmount = decimal.Divide(order.NetAmounts.TotalMoney?.Amount ?? 0, 100),
                           Timestamp = InstantPattern.ExtendedIso.Parse(order.CreatedAt).GetValueOrThrow(),
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
            var orderDate = InstantPattern.ExtendedIso.Parse(order.CreatedAt).GetValueOrThrow();
            var lineItems = order.LineItems.NullToEmpty().ToObservable().SelectMany(orderLineItem => this.CreateSubtransaction(orderLineItem, orderDate));
            var discounts = order.Discounts.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransactionFromDiscount);
            var returns = order.Returns.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransactionsFromReturn);
            var fees = order.ServiceCharges.NullToEmpty().ToObservable().SelectMany(serviceCharge => this.CreateSubtransactionFromServiceCharge(serviceCharge.Name, serviceCharge.TotalMoney));
            var tips = this.CreateSubtransactionFromTip(order.TotalTipMoney);
            return Observable.Merge(lineItems, discounts, returns, fees, tips).ToList();
        }

        private IObservable<Subtransaction> CreateSubtransaction(OrderLineItem orderLineItem, Instant orderDate)
        {
            return this.GetProductAsync(ProductId.Of(orderLineItem.CatalogObjectId), ProductName.Of(orderLineItem.Name))
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
            var productId = ResolveDiscountProductId();
            return Observable.FromAsync(() => this.productRepository.GetProductAsync(productId))
                             .Select(product =>
                             {
                                 var quantity = orderLineItemDiscount.AppliedMoney.Amount ?? 0;
                                 return new Subtransaction
                                 {
                                     Product = product,
                                     Quantity = (int)quantity,
                                     UnitPrice = -0.01M,
                                 };
                             });

            ProductId ResolveDiscountProductId()
            {
                if (orderLineItemDiscount.Name.Contains("macaron", StringComparison.OrdinalIgnoreCase))
                {
                    return ProductId.BunDiscountMacarons;
                }

                if (orderLineItemDiscount.Name.Contains("pocky", StringComparison.OrdinalIgnoreCase))
                {
                    return ProductId.BunDiscountPocky;
                }

                return ProductId.TlmDiscount;
            }
        }

        private IObservable<Subtransaction> CreateSubtransactionsFromReturn(OrderReturn orderReturn)
        {
            var lineItems = orderReturn.ReturnLineItems.NullToEmpty()
                                       .ToObservable()
                                       .SelectMany(async item =>
                                       {
                                           var product = await this.GetProductAsync(ProductId.Of(item.CatalogObjectId), ProductName.Of(item.Name));
                                           var quantity = -1 * int.Parse(item.Quantity);

                                           return new Subtransaction
                                           {
                                               Product = product,
                                               Quantity = quantity,
                                               UnitPrice = item.BasePriceMoney.ToDecimal(),
                                           };
                                       });

            var serviceCharges = orderReturn.ReturnServiceCharges.NullToEmpty()
                                            .ToObservable()
                                            .SelectMany(serviceCharge =>
                                            {
                                                var money = new Money(serviceCharge.TotalMoney.Amount * -1, serviceCharge.TotalMoney.Currency);
                                                return this.CreateSubtransactionFromServiceCharge(serviceCharge.Name, money);
                                            });

            return lineItems.Merge(serviceCharges);
        }

        private IObservable<Subtransaction> CreateSubtransactionFromServiceCharge(string serviceChargeName, Money serviceChargeMoney)
        {
            var productId = serviceChargeName?.ToUpper() switch
            {
                "SHIPPING" => ProductId.TlmDelivery,
                _ => ProductId.TlmUnknown,
            };

            return Observable.FromAsync(() => this.productRepository.GetProductAsync(productId))
                             .Select(product => SquareTransactionMapper.CreateSubtransactionFromMoney(product, serviceChargeMoney));
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

        private Task<Product> GetProductAsync(ProductId productId, ProductName name)
        {
            if (productId != null)
            {
                return this.productRepository.GetProductAsync(productId);
            }

            if (name != null)
            {
                return this.productRepository.GetProductAsync(name);
            }

            return this.productRepository.GetProductAsync(ProductId.TlmUnknown);
        }
    }
}
