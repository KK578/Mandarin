using Mandarin.Stockists;
using NodaTime;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a Product sold by The Little Mandarin.
    /// </summary>
    public record Product
    {
        /// <summary>
        /// Gets the unique product ID for this product.
        /// </summary>
        public ProductId ProductId { get; init; }

        /// <summary>
        /// Gets the Stockist ID for the stockist that owns this product.
        /// </summary>
        public StockistId StockistId { get; init; }

        /// <summary>
        /// Gets the unique internal product code for this product.
        /// </summary>
        public ProductCode ProductCode { get; init; }

        /// <summary>
        /// Gets the general name for this product.
        /// </summary>
        public ProductName ProductName { get; init; }

        /// <summary>
        /// Gets the detailed description for this product.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Gets the unit price for this product.
        /// </summary>
        public decimal? UnitPrice { get; init; }

        /// <summary>
        /// Gets the last time this product was updated.
        /// </summary>
        public Instant? LastUpdated { get; init; }

        /// <summary>
        /// Gets the user friendly string for this product.
        /// </summary>
        /// <returns>The user friendly string.</returns>
        public string FriendlyString() => $"[{this.ProductCode}] {this.ProductName}";
    }
}
