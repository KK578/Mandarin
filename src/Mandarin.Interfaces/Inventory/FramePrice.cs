using System;
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
        /// <param name="createdAt">The DateTime when the frame price was created.</param>
        /// <param name="activeUntil">The time up til which the frame price is active.</param>
        public FramePrice(string productCode, decimal amount, DateTime? createdAt = null, DateTime? activeUntil = null)
        {
            this.ProductCode = productCode;
            this.Amount = amount;
            this.CreatedAt = createdAt;
            this.ActiveUntil = activeUntil;
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

        /// <summary>
        /// Gets the time the entry was created.
        /// </summary>
        public DateTime? CreatedAt { get; }

        /// <summary>
        /// Gets the last time that the frame price is active til, or null.
        /// </summary>
        public DateTime? ActiveUntil { get; }
    }
}
