using Mandarin.Converters;
using Newtonsoft.Json;

namespace Mandarin.Models.Commissions
{
    /// <summary>
    /// Represents all sales for a specific product, with commission and sale amounts.
    /// </summary>
    public class Sale
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sale"/> class.
        /// </summary>
        /// <param name="productCode">The product's unique item code.</param>
        /// <param name="productName">The product's name.</param>
        /// <param name="quantity">The quantity of the product sold.</param>
        /// <param name="unitPrice">The unit price of the product sold.</param>
        /// <param name="subtotal">The total monetary amount sold before commission.</param>
        /// <param name="commission">The monetary amount that is commission.</param>
        /// <param name="total">The total monetary amount after commission is applied.</param>
        public Sale(string productCode,
                    string productName,
                    int quantity,
                    decimal unitPrice,
                    decimal subtotal,
                    decimal commission,
                    decimal total)
        {
            this.ProductCode = productCode;
            this.ProductName = productName;
            this.Quantity = quantity;
            this.UnitPrice = unitPrice;
            this.Subtotal = subtotal;
            this.Commission = commission;
            this.Total = total;
        }

        /// <summary>
        /// Gets the product's unique item code.
        /// </summary>
        [JsonProperty("productCode")]
        public string ProductCode { get; }

        /// <summary>
        /// Gets the product's name.
        /// </summary>
        [JsonProperty("productName")]
        public string ProductName { get; }

        /// <summary>
        /// Gets the quantity of products sold.
        /// </summary>
        [JsonProperty("quantity")]
        public int Quantity { get; }

        /// <summary>
        /// Gets the unit price for the product sold.
        /// </summary>
        [JsonProperty("unitPrice")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal UnitPrice { get; }

        /// <summary>
        /// Gets the total monetary amount of product sold before commission.
        /// </summary>
        [JsonProperty("subtotal")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal Subtotal { get; }

        /// <summary>
        /// Gets the total monetary amount that is commissioned of the product sold.
        /// </summary>
        [JsonProperty("commission")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal Commission { get; }

        /// <summary>
        /// Gets the total monetary amount of product sold after commission is applied.
        /// </summary>
        [JsonProperty("total")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal Total { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var commission = $"{nameof(Sale.Commission)}: {this.Commission:C} => {this.Total:C}";
            return $"{this.ProductCode}: {this.Quantity} * {this.UnitPrice:C} = {this.Subtotal} ({commission})";
        }
    }
}
