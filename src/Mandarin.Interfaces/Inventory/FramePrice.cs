using Newtonsoft.Json;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a partial amount on a sale of a framed product, where the frame price is considered as separate to the Artist's commission purposes.
    /// </summary>
    public class FramePrice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FramePrice"/> class.
        /// </summary>
        /// <param name="productCode">Product's unique item code.</param>
        /// <param name="amount">Monetary amount to be considered purely as commission.</param>
        public FramePrice(string productCode, decimal amount)
        {
            this.ProductCode = productCode;
            this.Amount = amount;
        }

        /// <summary>
        /// Gets the product's unique item code.
        /// </summary>
        [JsonProperty("product_code")]
        public string ProductCode { get; }

        /// <summary>
        /// Gets the product's frame price.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; }
    }
}
