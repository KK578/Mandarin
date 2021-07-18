namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a Product sold by The Little Mandarin.
    /// </summary>
    public record Product
    {
        /// <summary>
        /// Gets the unique product ID assigned by Square for this product.
        /// </summary>
        public ProductId SquareId { get; init; }

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
        /// Gets the user friendly string for this product.
        /// </summary>
        /// <returns>The user friendly string.</returns>
        public string FriendlyString() => $"[{this.ProductCode}] {this.ProductName}";
    }
}
