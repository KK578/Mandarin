using System.Collections.Generic;
using Mandarin.Converters;
using Newtonsoft.Json;
using NodaTime;

namespace Mandarin.Commissions
{
    /// <summary>
    /// Represents a summary of the sales for the specified artist (by code), with customizations for the Record of Sales email.
    /// </summary>
    public record RecordOfSales
    {
        /// <summary>
        /// Gets the artist's stockist code.
        /// </summary>
        [JsonProperty("stockistCode")]
        public string StockistCode { get; init; }

        /// <summary>
        /// Gets the artist's first name.
        /// </summary>
        [JsonIgnore]
        public string FirstName { get; init; }

        /// <summary>
        /// Gets the artist's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; init; }

        /// <summary>
        /// Gets the email address to send the Record of Sales to.
        /// </summary>
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; init; }

        /// <summary>
        /// Gets the custom message to attach to the Record of Sales.
        /// </summary>
        [JsonProperty("customMessage")]
        public string CustomMessage { get; init; }

        /// <summary>
        /// Gets the start date of transactions included in the Record of Sales.
        /// </summary>
        [JsonProperty("startDate")]
        public LocalDate StartDate { get; init; }

        /// <summary>
        /// Gets the end date of transactions included in the Record of Sales.
        /// </summary>
        [JsonProperty("endDate")]
        public LocalDate EndDate { get; init; }

        /// <summary>
        /// Gets the commission rate for the Record of Sales.
        /// </summary>
        [JsonProperty("rate")]
        [JsonConverter(typeof(NumberAsPercentageConverter))]
        public decimal Rate { get; init; }

        /// <summary>
        /// Gets the list of all sales in the Record of Sales.
        /// </summary>
        [JsonProperty("sales")]
        public IReadOnlyList<Sale> Sales { get; init; }

        /// <summary>
        /// Gets the total amount of money made by the sales (before commission).
        /// </summary>
        [JsonProperty("subtotal")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal Subtotal { get; init; }

        /// <summary>
        /// Gets the total amount of money to be paid as commission.
        /// </summary>
        [JsonProperty("commissionTotal")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal CommissionTotal { get; init; }

        /// <summary>
        /// Gets the total amount of money made in sales (after commission).
        /// </summary>
        [JsonProperty("total")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal Total { get; init; }

        /// <summary>
        /// Clones this instance of a <see cref="RecordOfSales"/> with modified email address and custom message if not null.
        /// </summary>
        /// <param name="emailAddress">New email address.</param>
        /// <param name="customMessage">New custom message.</param>
        /// <returns>Updated Artist Sales.</returns>
        public RecordOfSales WithMessageCustomisations(string emailAddress, string customMessage)
        {
            return this with
            {
                EmailAddress = emailAddress ?? this.EmailAddress,
                CustomMessage = customMessage ?? this.CustomMessage,
            };
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.StockistCode}: {this.Subtotal:C} @ {this.Rate:P} = {this.CommissionTotal:C}|{this.Total:C}";
        }
    }
}
