using Mandarin.Inventory;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents the purchase details of a particular product.
    /// </summary>
    public class Subtransaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subtransaction"/> class.
        /// </summary>
        /// <param name="product">The product represented in this subtransaction.</param>
        /// <param name="quantity">The quantity of the product that was sold as part of this subtransaction.</param>
        /// <param name="subtotal">The total monetary amount for this subtransaction.</param>
        public Subtransaction(Product product, int quantity, decimal subtotal)
        {
            this.Product = product;
            this.Quantity = quantity;
            this.Subtotal = subtotal;
            this.TransactionUnitPrice = quantity == 0 ? 0 : subtotal / quantity;
        }

        /// <summary>
        /// Gets the product that was sold in this subtransaction.
        /// </summary>
        public Product Product { get; }

        /// <summary>
        /// Gets the total quantity of the product that was sold.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Gets the unit price for the product within this subtransaction.
        /// This may differ from the product's unit price if the product has been updated since the transaction was made.
        /// </summary>
        public decimal TransactionUnitPrice { get; }

        /// <summary>
        /// Gets the total monetary amount for this subtransaction.
        /// </summary>
        public decimal Subtotal { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Product}: {this.TransactionUnitPrice:C} * {this.Quantity} = {this.Subtotal:C}";
        }
    }
}
