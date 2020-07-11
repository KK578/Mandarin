using Newtonsoft.Json;

namespace Mandarin.Models.Commissions
{
    /// <summary>
    /// Represents a partial amount on a sales that has a fixed portion of the sale as a separate fixed commission amount.
    /// </summary>
    public class FixedCommissionAmount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionAmount"/> class.
        /// </summary>
        /// <param name="productCode">Product's unique item code.</param>
        /// <param name="amount">Monetary amount to be considered purely as commission.</param>
        public FixedCommissionAmount(string productCode, decimal amount)
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
        /// Gets the product's fixed commission amount.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; }
    }
}
