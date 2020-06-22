using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Models.Inventory;
using Mandarin.Models.Transactions;
using Square.Models;
using Transaction = Mandarin.Models.Transactions.Transaction;

namespace Mandarin.Services.Square
{
    /// <inheritdoc />
    internal sealed class TransactionMapper : ITransactionMapper
    {
        private readonly IQueryableInventoryService inventoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionMapper"/> class.
        /// </summary>
        /// <param name="inventoryService">The inventory service.</param>
        public TransactionMapper(IQueryableInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
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
            var lineItems = order.LineItems.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransaction);
            var discounts = order.Discounts.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransactionFromDiscount);
            var returns = order.Returns.NullToEmpty().ToObservable().SelectMany(this.CreateSubtransactionsFromReturn);
            return Observable.Merge(lineItems, discounts, returns).ToList();
        }

        private IObservable<Subtransaction> CreateSubtransaction(OrderLineItem orderLineItem)
        {
            return this.GetProductAsync(orderLineItem.CatalogObjectId, orderLineItem.Name)
                       .ToObservable()
                       .SelectMany(product =>
                       {
                           return this.inventoryService.GetFixedCommissionAmount(product)
                                      .ToObservable()
                                      .SelectMany(fixedCommissionAmount => Create(product, fixedCommissionAmount));
                       });

            IEnumerable<Subtransaction> Create(Product product, FixedCommissionAmount fixedCommissionAmount)
            {
                if (fixedCommissionAmount != null)
                {
                    var quantity = int.Parse(orderLineItem.Quantity);
                    var commissionSubtotal = quantity * fixedCommissionAmount.Amount;
                    var subTotal = decimal.Divide(orderLineItem.TotalMoney?.Amount ?? 0, 100) - commissionSubtotal;

                    yield return new Subtransaction(product, quantity, subTotal);
                    yield return new Subtransaction(new Product("TLM-" + fixedCommissionAmount.ProductCode,
                                                                "TLM-" + fixedCommissionAmount.ProductCode,
                                                                $"Frame for {fixedCommissionAmount.ProductCode}",
                                                                null,
                                                                fixedCommissionAmount.Amount),
                                                    quantity,
                                                    commissionSubtotal);
                }
                else
                {
                    var quantity = int.Parse(orderLineItem.Quantity);
                    var subTotal = decimal.Divide(orderLineItem.TotalMoney?.Amount ?? 0, 100);
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

            var quantity = orderLineItemDiscount.AmountMoney.Amount ?? 0;
            var amount = decimal.Divide(quantity, 100);
            var subtransaction = new Subtransaction(product, (int)quantity, -amount);
            return Observable.Return(subtransaction);
        }

        private IObservable<Subtransaction> CreateSubtransactionsFromReturn(OrderReturn orderReturn)
        {
            return orderReturn.ReturnLineItems.ToObservable()
                              .SelectMany(async item =>
                              {
                                  var product = await this.GetProductAsync(item.CatalogObjectId, item.Name);
                                  var quantity = -1 * int.Parse(item.Quantity);
                                  var subtotal = decimal.Divide(item.TotalMoney?.Amount ?? 0, 100);
                                  return new Subtransaction(product, quantity, subtotal);
                              });
        }

        private Task<Product> GetProductAsync(string squareId, string name)
        {
            if (squareId != null)
            {
                return this.inventoryService.GetProductByIdAsync(squareId);
            }
            else if (name != null)
            {
                return this.inventoryService.GetProductByNameAsync(name);
            }
            else
            {
                var unknownProduct = new Product(null, "TLM-Unknown", "Unknown Product", "Unknown Product", null);
                return Task.FromResult(unknownProduct);
            }
        }
    }
}
