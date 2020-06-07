using System;
using System.Collections.Generic;
using System.Linq;
using Mandarin.Converters;
using Newtonsoft.Json;

namespace Mandarin.Models.Commissions
{
    /// <summary>
    /// Represents a summary of the sales for the specified artist (by code), with customizations for the Record of Sales email.
    /// </summary>
    public class ArtistSales
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistSales"/> class.
        /// </summary>
        /// <param name="stockistCode">The artist's unique stockist code.</param>
        /// <param name="name">The artist's full name.</param>
        /// <param name="emailAddress">Email address to send the Record of Sales to.</param>
        /// <param name="customMessage">Additional message to send in the Record of Sales to.</param>
        /// <param name="startDate">Start date of the sales summary.</param>
        /// <param name="endDate">End date of the sales summary.</param>
        /// <param name="rate">Commission rate for the sales summary.</param>
        /// <param name="sales">List of sales made between the start and end dates.</param>
        /// <param name="subtotal">Total amount of money made in sales (before commission).</param>
        /// <param name="commissionTotal">Total amount of money that is commissioned.</param>
        /// <param name="total">Total amount of money made in sales (after commission).</param>
        public ArtistSales(string stockistCode,
                           string name,
                           string emailAddress,
                           string customMessage,
                           DateTime startDate,
                           DateTime endDate,
                           decimal rate,
                           List<Sale> sales,
                           decimal subtotal,
                           decimal commissionTotal,
                           decimal total)
        {
            this.StockistCode = stockistCode;
            this.Name = name;
            this.EmailAddress = emailAddress;
            this.CustomMessage = customMessage;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Rate = rate;
            this.Sales = sales?.AsReadOnly();
            this.Subtotal = subtotal;
            this.CommissionTotal = commissionTotal;
            this.Total = total;
        }

        /// <summary>
        /// Gets the artist's stockist code for the sales summary.
        /// </summary>
        [JsonProperty("stockistCode")]
        public string StockistCode { get; }

        /// <summary>
        /// Gets the artist's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }

        /// <summary>
        /// Gets the email address to send the Record of Sales to.
        /// </summary>
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; }

        /// <summary>
        /// Gets the custom message to attach to the Record of Sales.
        /// </summary>
        [JsonProperty("customMessage")]
        public string CustomMessage { get; }

        /// <summary>
        /// Gets the start date of transactions for sales summary.
        /// </summary>
        [JsonProperty("startDate")]
        [JsonConverter(typeof(IsoDateConverter))]
        public DateTime StartDate { get; }

        /// <summary>
        /// Gets the end date of transactions for sales summary.
        /// </summary>
        [JsonProperty("endDate")]
        [JsonConverter(typeof(IsoDateConverter))]
        public DateTime EndDate { get; }

        /// <summary>
        /// Gets the commission rate for the sales summary.
        /// </summary>
        [JsonProperty("rate")]
        [JsonConverter(typeof(NumberAsPercentageConverter))]
        public decimal Rate { get; }

        /// <summary>
        /// Gets the list of all sales in this summary.
        /// </summary>
        [JsonProperty("sales")] public IReadOnlyList<Sale> Sales { get; }

        /// <summary>
        /// Gets the total amount of money made in sales (before commission).
        /// </summary>
        [JsonProperty("subtotal")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal Subtotal { get; }

        /// <summary>
        /// Gets the total amount of money that is commissioned.
        /// </summary>
        [JsonProperty("commissionTotal")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal CommissionTotal { get; }

        /// <summary>
        /// Gets the total amount of money made in sales (after commission).
        /// </summary>
        [JsonProperty("total")]
        [JsonConverter(typeof(NumberAsCurrencyConverter))]
        public decimal Total { get; }

        /// <summary>
        /// Clones this instance of a <see cref="ArtistSales"/> with modified email address and custom message if not null.
        /// </summary>
        /// <param name="emailAddress">New email address.</param>
        /// <param name="customMessage">New custom message.</param>
        /// <returns>Updated Artist Sales.</returns>
        public ArtistSales WithMessageCustomisations(string emailAddress, string customMessage)
        {
            return new ArtistSales(this.StockistCode,
                                   this.Name,
                                   emailAddress ?? this.EmailAddress,
                                   customMessage ?? this.CustomMessage,
                                   this.StartDate,
                                   this.EndDate,
                                   this.Rate,
                                   this.Sales?.ToList(),
                                   this.Subtotal,
                                   this.CommissionTotal,
                                   this.Total);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.StockistCode}: {this.Subtotal:C} @ {this.Rate:P} = {this.CommissionTotal:C}|{this.Total:C}";
        }
    }
}
