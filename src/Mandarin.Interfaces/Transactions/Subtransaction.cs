using Mandarin.Inventory;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents the purchase details of a particular product.
    /// </summary>
    public record Subtransaction
    {
        /// <summary>
        /// Gets the product that was sold in this subtransaction.
        /// </summary>
        public Product Product { get; init; }

        /// <summary>
        /// Gets the total quantity of the product that was sold.
        /// </summary>
        public int Quantity { get; init; }

        /// <summary>
        /// Gets the unit price for the product within this subtransaction.
        /// This may differ from the product's unit price if the product has been updated since the transaction was made.
        /// </summary>
        public decimal UnitPrice { get; init; }

        /// <summary>
        /// Gets the total monetary amount for this subtransaction.
        /// </summary>
        public decimal Subtotal => this.Quantity * this.UnitPrice;
    }
}
