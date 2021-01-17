namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a Product sold by The Little Mandarin.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="squareId">Unique product ID assigned by Square.</param>
        /// <param name="productCode">Unique internal product code.</param>
        /// <param name="productName">Name of this product.</param>
        /// <param name="description">Brief description of what this product is.</param>
        /// <param name="unitPrice">Monetary amount that this product sells for.</param>
        public Product(string squareId, string productCode, string productName, string description, decimal? unitPrice)
        {
            this.SquareId = squareId;
            this.ProductCode = productCode;
            this.ProductName = productName;
            this.Description = description;
            this.UnitPrice = unitPrice;
        }

        /// <summary>
        /// Gets the unique product ID assigned by Square for this product.
        /// </summary>
        public string SquareId { get; }

        /// <summary>
        /// Gets the unique internal product code for this product.
        /// </summary>
        public string ProductCode { get; }

        /// <summary>
        /// Gets the general name for this product.
        /// </summary>
        public string ProductName { get; }

        /// <summary>
        /// Gets the detailed description for this product.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the unit price for this product.
        /// </summary>
        public decimal? UnitPrice { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.ProductCode}: {this.ProductName}";
        }
    }
}
