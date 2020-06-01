using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Mandarin.Models.Inventory;
using Mandarin.Models.Transactions;
using Square.Models;
using Transaction = Mandarin.Models.Transactions.Transaction;

namespace Mandarin.Services.Square
{
    internal sealed class TransactionMapper : ITransactionMapper
    {
        private readonly IQueryableInventoryService inventoryService;

        public TransactionMapper(IQueryableInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
        }

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
            return this.GetProductAsync(orderLineItem.CatalogObjectId, orderLineItem.Name).ToObservable()
                       .Select(product =>
                       {
                           var quantity = int.Parse(orderLineItem.Quantity);
                           var subTotal = decimal.Divide(orderLineItem.TotalMoney?.Amount ?? 0, 100);
                           return new Subtransaction(product, quantity, subTotal);
                       });
        }

        private IObservable<Subtransaction> CreateSubtransactionFromDiscount(OrderLineItemDiscount orderLineItemDiscount)
        {
            Product product;

            if (orderLineItemDiscount.Name.Contains("macaron", StringComparison.OrdinalIgnoreCase))
            {
                product = new Product("BUN-DCM", "BUN-DCM", "Box of Macarons Discount", "Buy 6 macarons for \"£12.00\"", 0.01m);
            }
            else if (orderLineItemDiscount.Name.Contains("pocky", StringComparison.OrdinalIgnoreCase))
            {
                product = new Product("BUN-DCP", "BUN-DCP", "Box of Pocky Discount", "Discount on buying multiple packs of Pocky.", 0.01m);
            }
            else
            {
                product = new Product("TLM-D", "TLM-D", "Other discounts", "Discounts that aren't tracked.", 0.01m);
            }

            var quantity = (int)(orderLineItemDiscount.AmountMoney.Amount ?? 0);
            var subtransaction = new Subtransaction(product, quantity, orderLineItemDiscount.AmountMoney.Amount ?? 0);
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
            return squareId != null
                ? this.inventoryService.GetProductByIdAsync(squareId)
                : this.inventoryService.GetProductByNameAsync(name);
        }
    }
}