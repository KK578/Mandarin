using Mandarin.Converters;
using Newtonsoft.Json;

namespace Mandarin.Models.Commissions
{
    /// <summary>
    /// Represents all sales for a specific product, with commission and sale amounts.
    /// </summary>
    public record Sale
    {
        /// <summary>
        /// Gets the product's unique item code.
        /// </summary>
        [JsonProperty("productCode")]
        public string ProductCode { get; init; }

        /// <summary>
        /// Gets the product's name.
        /// </summary>
        [JsonProperty("productName")]
        public string ProductName { get; init; }

        /// <summary>
        /// Gets the quantity of products sold.
        /// </summary>
        [JsonProperty("quantity")]
        public int Quantity { get; init; }

        /// <summary>
        /// Gets the unit price for the product sold.
        /// </summary>
        [JsonProperty("unitPrice")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal UnitPrice { get; init; }

        /// <summary>
        /// Gets the total monetary amount of this product's sale (before commission).
        /// </summary>
        [JsonProperty("subtotal")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal Subtotal { get; init; }

        /// <summary>
        /// Gets the total monetary amount of this product's sale to be paid as commission.
        /// </summary>
        [JsonProperty("commission")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal Commission { get; init; }

        /// <summary>
        /// Gets the total monetary amount of this product's sale (after commission).
        /// </summary>
        [JsonProperty("total")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal Total { get; init; }

        /// <inheritdoc />
        public override string ToString()
        {
            var commission = $"{nameof(Sale.Commission)}: {this.Commission:C} => {this.Total:C}";
            return $"{this.ProductCode}: {this.Quantity} * {this.UnitPrice:C} = {this.Subtotal} ({commission})";
        }
    }
}
